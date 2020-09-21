using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Stateful;
using Unity.Transforms;
using static CharacterControllerUtilities;

[BurstCompile]
struct CharacterControllerJob : IJobChunk {
    private const float k_DefaultTau = 0.4f;
    private const float k_DefaultDamping = 0.9f;

    public float DeltaTime;

    [ReadOnly]
    public PhysicsWorld PhysicsWorld;

    public ComponentTypeHandle<CharacterControllerInternalData> CharacterControllerInternalType;
    public ComponentTypeHandle<CharacterControllerInput> CharacterControllerInputType;
    public ComponentTypeHandle<Translation> TranslationType;
    public ComponentTypeHandle<Rotation> RotationType;
    public BufferTypeHandle<StatefulCollisionEvent> CollisionEventBufferType;
    public BufferTypeHandle<StatefulTriggerEvent> TriggerEventBufferType;
    [ReadOnly] public ArchetypeChunkEntityType EntityType;
    [ReadOnly] public ComponentTypeHandle<CharacterControllerComponentData> CharacterControllerComponentType;
    [ReadOnly] public ComponentTypeHandle<PhysicsCollider> PhysicsColliderType;

    // Stores impulses we wish to apply to dynamic bodies the character is interacting with.
    // This is needed to avoid race conditions when 2 characters are interacting with the
    // same body at the same time.
    public NativeStream.Writer DeferredImpulseWriter;

    public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {
        float3 up = math.up();

        var chunkCCData = chunk.GetNativeArray(CharacterControllerComponentType);
        var chunkCCInternalData = chunk.GetNativeArray(CharacterControllerInternalType);
        var chunkCCInputData = chunk.GetNativeArray(CharacterControllerInputType);
        var chunkPhysicsColliderData = chunk.GetNativeArray(PhysicsColliderType);
        var chunkTranslationData = chunk.GetNativeArray(TranslationType);
        var chunkRotationData = chunk.GetNativeArray(RotationType);
        var entities = chunk.GetNativeArray(this.EntityType);

        var hasChunkCollisionEventBufferType = chunk.Has(CollisionEventBufferType);
        var hasChunkTriggerEventBufferType = chunk.Has(TriggerEventBufferType);

        BufferAccessor<StatefulCollisionEvent> collisionEventBuffers = default;
        BufferAccessor<StatefulTriggerEvent> triggerEventBuffers = default;
        if (hasChunkCollisionEventBufferType) {
            collisionEventBuffers = chunk.GetBufferAccessor(CollisionEventBufferType);
        }
        if (hasChunkTriggerEventBufferType) {
            triggerEventBuffers = chunk.GetBufferAccessor(TriggerEventBufferType);
        }

        DeferredImpulseWriter.BeginForEachIndex(chunkIndex);

        for (int i = 0; i < chunk.Count; i++) {
            var ccComponentData = chunkCCData[i];
            var ccInternalData = chunkCCInternalData[i];
            var ccInput = chunkCCInputData[i];
            var collider = chunkPhysicsColliderData[i];
            var position = chunkTranslationData[i];
            var rotation = chunkRotationData[i];
            DynamicBuffer<StatefulCollisionEvent> collisionEventBuffer = default;
            DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer = default;

            if (hasChunkCollisionEventBufferType) {
                collisionEventBuffer = collisionEventBuffers[i];
            }

            if (hasChunkTriggerEventBufferType) {
                triggerEventBuffer = triggerEventBuffers[i];
            }

            // Collision filter must be valid
            if (!collider.IsValid || collider.Value.Value.Filter.IsEmpty)
                continue;

            // Character step input
            CharacterControllerStepInput stepInput = new CharacterControllerStepInput {
                World = PhysicsWorld,
                DeltaTime = DeltaTime,
                Up = math.up(),
                Gravity = ccComponentData.Gravity,
                MaxIterations = ccComponentData.MaxIterations,
                Tau = k_DefaultTau,
                Damping = k_DefaultDamping,
                SkinWidth = ccComponentData.SkinWidth,
                ContactTolerance = ccComponentData.ContactTolerance,
                MaxSlope = ccComponentData.MaxSlope,
                RigidBodyIndex = this.PhysicsWorld.GetRigidBodyIndex(entities[i]),
                CurrentVelocity = ccInternalData.LinearVelocity,
                MaxMovementSpeed = ccComponentData.MaxMovementSpeed
            };

            // Character transform
            RigidTransform transform = new RigidTransform {
                pos = position.Value,
                rot = rotation.Value
            };

            NativeList<StatefulCollisionEvent> currentFrameCollisionEvents = default;
            NativeList<StatefulTriggerEvent> currentFrameTriggerEvents = default;

            if (ccComponentData.RaiseCollisionEvents != 0) {
                currentFrameCollisionEvents = new NativeList<StatefulCollisionEvent>(Allocator.Temp);
            }

            if (ccComponentData.RaiseTriggerEvents != 0) {
                currentFrameTriggerEvents = new NativeList<StatefulTriggerEvent>(Allocator.Temp);
            }


            // Check support
            CheckSupport(ref PhysicsWorld, ref collider, stepInput, transform,
                out ccInternalData.SupportedState, out float3 surfaceNormal, out float3 surfaceVelocity,
                currentFrameCollisionEvents);

            // User input
            float3 desiredVelocity = ccInternalData.LinearVelocity;
            HandleUserInput(ccComponentData, stepInput.Up, surfaceVelocity, ref ccInternalData, ref desiredVelocity, ref ccInput);

            // Calculate actual velocity with respect to surface
            if (ccInternalData.SupportedState == CharacterSupportState.Supported) {
                CalculateMovement(ccInternalData.CurrentRotationAngle, stepInput.Up, ccInternalData.IsJumping,
                    ccInternalData.LinearVelocity, desiredVelocity, surfaceNormal, surfaceVelocity, out ccInternalData.LinearVelocity);
            } else {
                ccInternalData.LinearVelocity = desiredVelocity;
            }

            // World collision + integrate
            CollideAndIntegrate(stepInput, ccComponentData.CharacterMass, ccComponentData.AffectsPhysicsBodies != 0,
                collider.ColliderPtr, ref transform, ref ccInternalData.LinearVelocity, ref DeferredImpulseWriter,
                currentFrameCollisionEvents, currentFrameTriggerEvents);

            // Update collision event status
            if (currentFrameCollisionEvents.IsCreated) {
                UpdateCollisionEvents(currentFrameCollisionEvents, collisionEventBuffer);
            }

            if (currentFrameTriggerEvents.IsCreated) {
                UpdateTriggerEvents(currentFrameTriggerEvents, triggerEventBuffer);
            }

            // Write back and orientation integration
            position.Value = transform.pos;
            rotation.Value = quaternion.AxisAngle(up, ccInternalData.CurrentRotationAngle);

            // Write back to chunk data
            {
                chunkCCInternalData[i] = ccInternalData;
                chunkTranslationData[i] = position;
                chunkRotationData[i] = rotation;
            }
        }

        DeferredImpulseWriter.EndForEachIndex();
    }

    private void HandleUserInput(
        CharacterControllerComponentData ccComponentData, float3 up, float3 surfaceVelocity,
        ref CharacterControllerInternalData ccInternalData, ref float3 linearVelocity,
        ref CharacterControllerInput ccInput) {
        // Reset jumping state and unsupported velocity
        if (ccInternalData.SupportedState == CharacterSupportState.Supported) {
            ccInternalData.IsJumping = false;
            ccInternalData.UnsupportedVelocity = float3.zero;
        }

        float3 requestedMovementDirection = float3.zero;

        // Movement and jumping
        bool shouldJump;
        {
            float3 forward = math.forward(quaternion.identity);
            float3 right = math.cross(up, forward);

            float horizontal = ccInput.Movement.x;
            float vertical = ccInput.Movement.y;
            bool jumpRequested = ccInput.Jumped > 0;
            bool haveInput = (math.abs(horizontal) > float.Epsilon) || (math.abs(vertical) > float.Epsilon);
            if (haveInput) {
                float3 localSpaceMovement = forward * vertical + right * horizontal;
                float3 worldSpaceMovement = math.rotate(quaternion.AxisAngle(up, ccInternalData.CurrentRotationAngle), localSpaceMovement);
                requestedMovementDirection = worldSpaceMovement;
            }
            shouldJump = jumpRequested && ccInternalData.SupportedState == CharacterSupportState.Supported;
        }

        // Apply input velocities
        {
            if (shouldJump) {
                // Add jump speed to surface velocity and make character unsupported
                ccInternalData.IsJumping = true;
                ccInternalData.SupportedState = CharacterSupportState.Unsupported;
                ccInternalData.UnsupportedVelocity = surfaceVelocity + ccComponentData.JumpUpwardsSpeed * up;
            } else if (ccInternalData.SupportedState != CharacterSupportState.Supported) {
                // Apply gravity
                ccInternalData.UnsupportedVelocity += ccComponentData.Gravity * DeltaTime;
            }
            // If unsupported then keep jump and surface momentum
            linearVelocity = requestedMovementDirection +
                (ccInternalData.SupportedState != CharacterSupportState.Supported ? ccInternalData.UnsupportedVelocity : float3.zero);
        }
    }

    private void CalculateMovement(float currentRotationAngle, float3 up, bool isJumping,
        float3 currentVelocity, float3 desiredVelocity, float3 surfaceNormal, float3 surfaceVelocity, out float3 linearVelocity) {
        float3 forward = math.forward(quaternion.AxisAngle(up, currentRotationAngle));

        Rotation surfaceFrame;
        float3 binorm;
        {
            binorm = math.cross(forward, up);
            binorm = math.normalize(binorm);

            float3 tangent = math.cross(binorm, surfaceNormal);
            tangent = math.normalize(tangent);

            binorm = math.cross(tangent, surfaceNormal);
            binorm = math.normalize(binorm);

            surfaceFrame.Value = new quaternion(new float3x3(binorm, tangent, surfaceNormal));
        }

        float3 relative = currentVelocity - surfaceVelocity;
        relative = math.rotate(math.inverse(surfaceFrame.Value), relative);

        float3 diff;
        {
            float3 sideVec = math.cross(forward, up);
            float fwd = math.dot(desiredVelocity, forward);
            float side = math.dot(desiredVelocity, sideVec);
            float len = math.length(desiredVelocity);
            float3 desiredVelocitySF = new float3(-side, -fwd, 0.0f);
            desiredVelocitySF = math.normalizesafe(desiredVelocitySF, float3.zero);
            desiredVelocitySF *= len;
            diff = desiredVelocitySF - relative;
        }

        relative += diff;

        linearVelocity = math.rotate(surfaceFrame.Value, relative) + surfaceVelocity +
            (isJumping ? math.dot(desiredVelocity, up) * up : float3.zero);
    }

    private void UpdateTriggerEvents(NativeList<StatefulTriggerEvent> triggerEvents,
        DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer) {
        triggerEvents.Sort();

        var previousFrameTriggerEvents = new NativeList<StatefulTriggerEvent>(triggerEventBuffer.Length, Allocator.Temp);

        for (int i = 0; i < triggerEventBuffer.Length; i++) {
            var triggerEvent = triggerEventBuffer[i];
            if (triggerEvent.State != EventOverlapState.Exit) {
                previousFrameTriggerEvents.Add(triggerEvent);
            }
        }

        var eventsWithState = new NativeList<StatefulTriggerEvent>(triggerEvents.Length, Allocator.Temp);

        TriggerEventConversionSystem.UpdateTriggerEventState(previousFrameTriggerEvents, triggerEvents, eventsWithState);

        triggerEventBuffer.Clear();

        for (int i = 0; i < eventsWithState.Length; i++) {
            triggerEventBuffer.Add(eventsWithState[i]);
        }
    }

    private void UpdateCollisionEvents(NativeList<StatefulCollisionEvent> collisionEvents,
        DynamicBuffer<StatefulCollisionEvent> collisionEventBuffer) {
        collisionEvents.Sort();

        var previousFrameCollisionEvents = new NativeList<StatefulCollisionEvent>(collisionEventBuffer.Length, Allocator.Temp);

        for (int i = 0; i < collisionEventBuffer.Length; i++) {
            var collisionEvent = collisionEventBuffer[i];
            if (collisionEvent.CollidingState != EventCollidingState.Exit) {
                previousFrameCollisionEvents.Add(collisionEvent);
            }
        }

        var eventsWithState = new NativeList<StatefulCollisionEvent>(collisionEvents.Length, Allocator.Temp);

        CollisionEventConversionSystem.UpdateCollisionEventState(previousFrameCollisionEvents, collisionEvents, eventsWithState);

        collisionEventBuffer.Clear();

        for (int i = 0; i < eventsWithState.Length; i++) {
            collisionEventBuffer.Add(eventsWithState[i]);
        }
    }
}

using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using static CharacterControllerUtilities;
using static Unity.Physics.PhysicsStep;

[Serializable]
public struct CharacterRigidbody : IComponentData {
    public float3 GroundProbeVector;
    public float MaxSlope; // radians
    public int MaxIterations;
    public float CharacterMass;
    public float SkinWidth;
    public float ContactTolerance;
    public int AffectsPhysicsBodies;
    public float MaxMovementSpeed;
    public float3 CapsuleCenter;
    public float CapsuleRadius;
    public float CapsuleHeight;
}

public struct GroundContactStatus : IComponentData {
    public float3 SurfaceNormal;
    public float3 SurfaceVelocity;
    public CharacterSupportState SupportedState;
}

public struct CharacterPhysicsInput : IComponentData {
    public float3 StartPosition;
    public bool FollowGround;
    public bool CheckSupport;
}

public struct CharacterPhysicsOutput : IComponentData {
    public float3 MoveResult;
}

public struct CharacterPhysicsVelocity : IComponentData {
    public float3 Velocity;
}

struct CharacterControllerCollider : ISystemStateComponentData {
    public BlobAssetReference<Unity.Physics.Collider> Collider;
}

[Serializable]
public class CharacterPhysicsBody : MonoBehaviour, IConvertGameObjectToEntity {
    public float3 Gravity = Default.Gravity;
    public float MaxSlope = 60.0f;
    // Maximum number of character controller solver iterations
    public int MaxIterations = 10;
    public float CharacterMass = 1.0f;
    // Keep the character at this distance to planes (used for numerical stability)
    public float SkinWidth = 0.02f;
    // Anything in this distance to the character will be considered a potential contact
    // when checking support
    public float ContactTolerance = 0.1f;
    public int AffectsPhysicsBodies = 1;
    public float MaxMovementSpeed = 10.0f;
    public float3 GroundProbeVector = new float3(0.0f, -0.1f, 0.0f);
    public float3 CapsuleCenter = new float3(0.0f, 1.0f, 0.0f);
    public float CapsuleRadius = 0.5f;
    public float CapsuleHeight = 2.0f;

    void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        if (this.enabled) {
            dstManager.AddComponentData(entity, new CharacterRigidbody {
                GroundProbeVector = GroundProbeVector,
                MaxSlope = math.radians(this.MaxSlope),
                MaxIterations = MaxIterations,
                CharacterMass = CharacterMass,
                SkinWidth = SkinWidth,
                ContactTolerance = ContactTolerance,
                AffectsPhysicsBodies = AffectsPhysicsBodies,
                MaxMovementSpeed = MaxMovementSpeed,
                CapsuleCenter = CapsuleCenter,
                CapsuleHeight = CapsuleHeight,
                CapsuleRadius = CapsuleRadius
            });
            dstManager.AddComponent(entity, typeof(CharacterPhysicsVelocity));
            dstManager.AddComponent(entity, typeof(CharacterPhysicsInput));
            dstManager.AddComponent(entity, typeof(CharacterPhysicsOutput));
            dstManager.AddComponent(entity, typeof(GroundContactStatus));
        }
    }
}


[AlwaysSynchronizeSystem]
[AlwaysUpdateSystem]
[UpdateBefore(typeof(CharacterControllerStepSystem))]
public class CharacterControllerInitAndCleanupSystem : JobComponentSystem {
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        this.Entities
            .WithNone<CharacterControllerCollider>()
            .ForEach((Entity e, ref CharacterRigidbody initData) => {
                var capsule = new CapsuleGeometry {
                    Vertex0 = initData.CapsuleCenter + new float3(0, 0.5f * initData.CapsuleHeight - initData.CapsuleRadius, 0),
                    Vertex1 = initData.CapsuleCenter - new float3(0, 0.5f * initData.CapsuleHeight - initData.CapsuleRadius, 0),
                    Radius = initData.CapsuleRadius
                };
                var filter = new CollisionFilter { BelongsTo = 1, CollidesWith = 1, GroupIndex = 0 };
                var collider = Unity.Physics.CapsuleCollider.Create(capsule, filter, new Unity.Physics.Material { Flags = new Unity.Physics.Material.MaterialFlags() });
                ecb.AddComponent(e, new CharacterControllerCollider { Collider = collider });
            }).Run();

        this.Entities
            .WithNone<CharacterRigidbody>()
            .ForEach((Entity e, ref CharacterControllerCollider collider) => {
                collider.Collider.Dispose();
                ecb.RemoveComponent<CharacterControllerCollider>(e);
            }).Run();

        ecb.Playback(this.EntityManager);
        ecb.Dispose();

        return inputDeps;
    }
}
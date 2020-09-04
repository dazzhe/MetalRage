using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(ExportPhysicsWorld)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public class CharacterControllerSystem : SystemBase {

    public JobHandle OutDependency => Dependency;

    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    ExportPhysicsWorld m_ExportPhysicsWorldSystem;
    EndFramePhysicsSystem m_EndFramePhysicsSystem;

    EntityQuery m_CharacterControllersGroup;

    protected override void OnCreate() {
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<ExportPhysicsWorld>();
        m_EndFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();

        EntityQueryDesc query = new EntityQueryDesc {
            All = new ComponentType[]
            {
                typeof(CharacterControllerComponentData),
                typeof(CharacterControllerInternalData),
                typeof(PhysicsCollider),
                typeof(Translation),
                typeof(Rotation),
            }
        };
        m_CharacterControllersGroup = GetEntityQuery(query);
    }

    protected override void OnUpdate() {
        if (m_CharacterControllersGroup.CalculateEntityCount() == 0)
            return;

        var chunks = m_CharacterControllersGroup.CreateArchetypeChunkArray(Allocator.TempJob);
        var deferredImpulses = new NativeStream(chunks.Length, Allocator.TempJob);

        var ccJob = new CharacterControllerJob {
            // Archetypes
            CharacterControllerComponentType = GetComponentTypeHandle<CharacterControllerComponentData>(),
            CharacterControllerInternalType = GetComponentTypeHandle<CharacterControllerInternalData>(),
            CharacterControllerInputType = GetComponentTypeHandle<CharacterControllerInput>(),
            PhysicsColliderType = GetComponentTypeHandle<PhysicsCollider>(),
            TranslationType = GetComponentTypeHandle<Translation>(),
            RotationType = GetComponentTypeHandle<Rotation>(),
            EntityType = GetArchetypeChunkEntityType(),
            CollisionEventBufferType = GetBufferTypeHandle<StatefulCollisionEvent>(),
            TriggerEventBufferType = GetBufferTypeHandle<StatefulTriggerEvent>(),

            // Input
            DeltaTime = UnityEngine.Time.fixedDeltaTime,
            PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld,
            DeferredImpulseWriter = deferredImpulses.AsWriter()
        };

        Dependency = JobHandle.CombineDependencies(Dependency, m_ExportPhysicsWorldSystem.GetOutputDependency());
        Dependency = ccJob.Schedule(m_CharacterControllersGroup, Dependency);

        var applyJob = new ApplyDefferedPhysicsUpdatesJob() {
            Chunks = chunks,
            DeferredImpulseReader = deferredImpulses.AsReader(),
            PhysicsVelocityData = GetComponentDataFromEntity<PhysicsVelocity>(),
            PhysicsMassData = GetComponentDataFromEntity<PhysicsMass>(),
            TranslationData = GetComponentDataFromEntity<Translation>(),
            RotationData = GetComponentDataFromEntity<Rotation>()
        };

        Dependency = applyJob.Schedule(Dependency);
        var disposeHandle = deferredImpulses.Dispose(Dependency);

        // Must finish all jobs before physics step end
        m_EndFramePhysicsSystem.AddInputDependency(disposeHandle);
    }

#if !UNITY_ENTITIES_0_12_OR_NEWER
    BufferTypeHandle<T> GetBufferTypeHandle<T>(bool isReadOnly = false) where T : struct, IBufferElementData => new BufferTypeHandle<T> { Value = GetArchetypeChunkBufferType<T>(isReadOnly) };
    ComponentTypeHandle<T> GetComponentTypeHandle<T>(bool isReadOnly = false) where T : struct, IComponentData => new ComponentTypeHandle<T> { Value = GetArchetypeChunkComponentType<T>(isReadOnly) };
#endif
}

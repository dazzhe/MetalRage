using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

[BurstCompile]
struct ApplyDefferedPhysicsUpdatesJob : IJob {
    // Chunks can be deallocated at this point
    [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;

    public NativeStream.Reader DeferredImpulseReader;

    public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityData;
    public ComponentDataFromEntity<PhysicsMass> PhysicsMassData;
    public ComponentDataFromEntity<Translation> TranslationData;
    public ComponentDataFromEntity<Rotation> RotationData;

    public void Execute() {
        int index = 0;
        int maxIndex = DeferredImpulseReader.ForEachCount;
        DeferredImpulseReader.BeginForEachIndex(index++);
        while (DeferredImpulseReader.RemainingItemCount == 0 && index < maxIndex) {
            DeferredImpulseReader.BeginForEachIndex(index++);
        }

        while (DeferredImpulseReader.RemainingItemCount > 0) {
            // Read the data
            var impulse = DeferredImpulseReader.Read<DeferredCharacterControllerImpulse>();
            while (DeferredImpulseReader.RemainingItemCount == 0 && index < maxIndex) {
                DeferredImpulseReader.BeginForEachIndex(index++);
            }

            PhysicsVelocity pv = PhysicsVelocityData[impulse.Entity];
            PhysicsMass pm = PhysicsMassData[impulse.Entity];
            Translation t = TranslationData[impulse.Entity];
            Rotation r = RotationData[impulse.Entity];

            // Don't apply on kinematic bodies
            if (pm.InverseMass > 0.0f) {
                // Apply impulse
                pv.ApplyImpulse(pm, t, r, impulse.Impulse, impulse.Point);

                // Write back
                PhysicsVelocityData[impulse.Entity] = pv;
            }
        }
    }
}


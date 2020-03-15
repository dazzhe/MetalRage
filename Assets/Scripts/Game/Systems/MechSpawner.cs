using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MechSpawner : ComponentSystem {
    private EntityQuery mechPrefabQuery;

    protected override void OnCreate() {
        this.mechPrefabQuery = this.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<MechPrefabEntry>());
    }

    protected override void OnUpdate() {
        this.Entities.ForEach((Entity entity, ref MechSpawnRequest request) => {
            this.PostUpdateCommands.DestroyEntity(entity);
            var prefabEntity = this.EntityManager.GetBuffer<MechPrefabEntry>(this.mechPrefabQuery.GetSingletonEntity())[0].Prefab;
            var spawnedEntity = this.EntityManager.Instantiate(prefabEntity);
            var mech = this.EntityManager.GetComponentData<Mech>(spawnedEntity);
            this.EntityManager.SetComponentData(spawnedEntity, new Translation { Value = request.Position });
            this.EntityManager.SetComponentData(spawnedEntity, new Rotation { Value = request.Rotation });
            this.PostUpdateCommands.DestroyEntity(entity);
        });
    }
}

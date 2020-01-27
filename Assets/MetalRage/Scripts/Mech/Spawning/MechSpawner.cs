using Unity.Entities;
using UnityEngine;

public class MechSpawner : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((Entity entity, ref MechSpawnRequest request) => {
            SpawnMech(request);
            this.PostUpdateCommands.DestroyEntity(entity);
        });
    }

    private void SpawnMech(MechSpawnRequest request) {
        var mechConfigMap = Game.Config.GetConfig<MechConfigMap>();
        var mechConfig = mechConfigMap[request.MechType];
        var obj = Object.Instantiate(mechConfig.Prefab, request.Position, request.Rotation);
        var entity = obj.GetComponent<GameObjectEntity>();
        this.PostUpdateCommands.AddComponent(entity.Entity, new MechMovementStatus());
        this.PostUpdateCommands.AddComponent(entity.Entity, new BoosterEngineStatus { Gauge = 100 });
        this.PostUpdateCommands.AddComponent(entity.Entity, new BoostActionConfigData { MaxSpeed = 30f, Consumption = 1, Duration = 0.2f });
        this.PostUpdateCommands.AddComponent(entity.Entity, mechConfig.Movement);
    }
}

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
        var actions = mechConfig.Actions;
        var actionBuffer = this.PostUpdateCommands.AddBuffer<MechActionEntity>(entity.Entity);
        for (int i = 0; i < actions.Length; ++i) {
            var action = actions[i].CreateBufferElement(this.EntityManager, entity.Entity);
            actionBuffer.Add(action);
        }
        this.PostUpdateCommands.AddComponent(entity.Entity, new MechLocoStatus());
        this.PostUpdateCommands.AddComponent(entity.Entity, new MechLocoCommand());
        this.PostUpdateCommands.AddComponent(entity.Entity, new Unity.Transforms.CompositeRotation());
    }
}

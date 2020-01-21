using Unity.Entities;
using UnityEngine;

public class MechSpawner : ComponentSystem {
    private ComponentGroup group;

    protected override void OnCreateManager() {
        base.OnCreateManager();
        this.group = GetComponentGroup(typeof(MechSpawnRequest));
    }

    protected override void OnUpdate() {
        var requests = this.group.GetComponentDataArray<MechSpawnRequest>();
        if (requests.Length == 0) {
            return;
        }
        var requestEntities = this.group.GetEntityArray();
        // Copy requests as spawning will invalidate Group
        var copiedRequests = new MechSpawnRequest[requests.Length];
        for (var i = 0; i < requests.Length; i++) {
            copiedRequests[i] = requests[i];
            this.PostUpdateCommands.DestroyEntity(requestEntities[i]);
        }
        for (var i = 0; i < copiedRequests.Length; i++) {
            var request = copiedRequests[i];
            SpawnMech(request);
        }
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
    }
}

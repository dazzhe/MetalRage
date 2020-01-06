using Unity.Entities;
using UnityEngine;

public class MechSpawnRequestHandler : ComponentSystem {
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
        var mechPrefabMap = Game.Config.GetConfig<MechPrefabMap>();
        var mechPrefab = mechPrefabMap[request.MechType];
        //var obj = Object.Instantiate(mechPrefab, request.Position, request.Rotation);
        //var entity = obj.GetComponent<GameObjectEntity>();
    }
}
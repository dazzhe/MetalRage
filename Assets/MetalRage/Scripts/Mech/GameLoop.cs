using Unity.Entities;
using UnityEngine;

public class GameLoopSystem : ComponentSystem {
    private bool isInitialized = false;

    protected override void OnUpdate() {
        if (!this.isInitialized) {
            var entity = this.EntityManager.CreateEntity();
            MechSpawnRequest.Create(this.PostUpdateCommands, MechType.Vanguard, Vector3.zero, Quaternion.identity, entity);
        }
    }
}

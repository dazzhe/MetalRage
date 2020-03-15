using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class GameLoopSystem : ComponentSystem {
    private EntityQuery mechQuery;
    private EntityQuery cameraCommandQuery;

    protected override void OnCreate() {
        this.mechQuery = this.EntityManager.CreateEntityQuery(typeof(Mech));
        this.cameraCommandQuery = this.EntityManager.CreateEntityQuery(typeof(PlayerCameraCommand));
    }

    protected override void OnUpdate() {
        var mechArray = this.mechQuery.ToEntityArray(Allocator.TempJob);
        if (mechArray.Length == 0) {
            var entity = this.EntityManager.CreateEntity();
            MechSpawnRequest.Create(this.PostUpdateCommands, MechType.Vanguard, Vector3.up, Quaternion.identity, entity);
        } else {
            if (this.cameraCommandQuery.CalculateEntityCount() == 0) {
                this.cameraCommandQuery.SetSingleton(new PlayerCameraCommand());
            }
            var command = this.cameraCommandQuery.GetSingleton<PlayerCameraCommand>();
            command.TargetEntity = mechArray[0];
            this.cameraCommandQuery.SetSingleton(command);
        }
        mechArray.Dispose();
    }
}

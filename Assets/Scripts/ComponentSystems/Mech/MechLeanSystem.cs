using Unity.Entities;
using UnityEngine;

public struct PlayerCameraCommand : IComponentData {
    public Bool LeanLeft;
    public Bool LeanRight;
    public Bool CancelLean;
}

[UpdateAfter(typeof(PlayerCameraSystem))]
[UpdateBefore(typeof(PlayerCameraSystem))]
public class MechLeanSystem : ComponentSystem {
    private EntityQuery query;

    protected override void OnCreate() {
        this.query = GetEntityQuery(typeof(PlayerCameraCommand));
    }

    protected override void OnUpdate() {
        if (this.query.CalculateEntityCount() == 0) {
            this.EntityManager.CreateEntity(typeof(PlayerCameraCommand));
        }
        var command = this.query.GetSingleton<PlayerCameraCommand>();
        this.Entities.ForEach((ref PlayerInputData input, ref MechMovementStatus movement) => {

            input.LeanLeft
        });
    }
}

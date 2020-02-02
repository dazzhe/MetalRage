using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(MechInputSystem))]
[UpdateBefore(typeof(PlayerCameraSystem))]
public class MechCameraControlSystem : ComponentSystem {
    private EntityQuery query;

    protected override void OnCreate() {
        this.query = GetEntityQuery(typeof(PlayerCameraCommand));
    }

    protected override void OnUpdate() {
        if (this.query.CalculateEntityCount() == 0) {
            return;
            //this.EntityManager.CreateEntity(typeof(PlayerCameraCommand));
        }
        var command = new PlayerCameraCommand();
        this.Entities.ForEach((ref PlayerInputData input, ref MechMovementStatus movement) => {
            var mechIsStable = movement.Velocity == Vector3.zero;
            if (input.LeanLeft && mechIsStable) {
                command.LeanLeft = true;
            } else if (input.LeanRight && mechIsStable) {
                command.LeanRight = true;
            } else if (!input.LeanLeft && !input.LeanRight) {
                command.CancelLean = true;
            }
        });
        this.query.SetSingleton(command);
    }
}

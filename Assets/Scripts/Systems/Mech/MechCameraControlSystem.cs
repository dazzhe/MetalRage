using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(MechInputSystem))]
[UpdateBefore(typeof(PlayerCameraSystem))]
public class MechCameraControlSystem : ComponentSystem {
    private EntityQuery cameraQuery;
    private EntityQuery inputQuery;

    protected override void OnCreate() {
        this.cameraQuery = GetEntityQuery(typeof(PlayerCameraCommand));
        this.inputQuery = GetEntityQuery(typeof(PlayerInputData));
    }

    protected override void OnUpdate() {
        var input = this.inputQuery.GetSingleton<PlayerInputData>();
        if (this.cameraQuery.CalculateEntityCount() == 0) {
            return;
            //this.EntityManager.CreateEntity(typeof(PlayerCameraCommand));
        }
        var command = new PlayerCameraCommand();
        this.Entities.ForEach((Transform transform, ref Mech mech, ref MechMovementStatus movement) => {
            command.TargetPosition = transform.TransformPoint(mech.CameraTargetTranslation);
            command.TargetRotation = Quaternion.Euler(movement.Pitch, movement.Yaw, 0f);
            var mechIsStable = movement.Velocity == Vector3.zero;
            if (input.LeanLeft && mechIsStable) {
                command.LeanLeft = true;
            } else if (input.LeanRight && mechIsStable) {
                command.LeanRight = true;
            } else if (!input.LeanLeft && !input.LeanRight) {
                command.CancelLean = true;
            }
        });
        this.cameraQuery.SetSingleton(command);
    }
}

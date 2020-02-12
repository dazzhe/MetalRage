using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(PlayerInputSystem)), UpdateBefore(typeof(PlayerCameraSystem))]
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
            command.MaxDistance = 1.5f;
            if (input.LeanLeft && mechIsStable) {
                command.RequestedMode = CameraMode.LeanLeft;
            } else if (input.LeanRight && mechIsStable) {
                command.RequestedMode = CameraMode.LeanRight;
            } else if (!input.LeanLeft && !input.LeanRight) {
                command.RequestedMode = CameraMode.SmoothFollow;
                if (movement.State == MechMovementState.Braking) {
                    command.MaxSpeed = 30f;
                } else if (movement.State == MechMovementState.Idle || movement.State == MechMovementState.Crouching) {
                    command.MaxSpeed = 5f;
                } else {
                    command.MaxSpeed = 1f;
                }
            } else {
                command.RequestedMode = CameraMode.None;
            }
        });
        this.cameraQuery.SetSingleton(command);
    }
}

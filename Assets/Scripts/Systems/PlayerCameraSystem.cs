using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public enum CameraLeanState {
    Right,
    Left,
    Off
}

public struct PlayerCameraCommand : IComponentData {
    public Bool LeanLeft;
    public Bool LeanRight;
    public Bool CancelLean;
    public Vector3 TargetPosition;
    public Quaternion TargetRotation;
}

public struct CameraLeanStatus {
    public CameraLeanState State;
    public Vector3 currentLeanOffset;
}

public struct PlayerCamera : IComponentData {
    public CameraLeanStatus LeanStatus;
    public Vector3 BaseCameraOffset;
    public float leanLength;// = 5f;
    public float forwardOffsetFactor;// = 4.7f;
}

[UpdateAfter(typeof(MechMovementSystem))]
public class PlayerCameraSystem : ComponentSystem {
    private EntityQuery query;

    protected override void OnCreate() {
        this.query = GetEntityQuery(
            typeof(PlayerCamera), typeof(PlayerCameraCommand), typeof(Translation),
            typeof(Rotation));
    }

    protected override void OnUpdate() {
        this.Entities.With(this.query).ForEach((ref PlayerCamera playerCamera, ref PlayerCameraCommand command, ref Translation translation, ref Rotation rotation) => {
            var pitch = command.TargetRotation.eulerAngles.x * Mathf.Deg2Rad;
            var offsetLength = Mathf.Abs(Mathf.Sin(pitch)) * playerCamera.forwardOffsetFactor;
            var cameraOffset = playerCamera.BaseCameraOffset + offsetLength * Vector3.forward;
            if (command.LeanLeft) {
                //            camera.Lea
            }

            //SetLeanType();
            if (command.CancelLean) {
                playerCamera.LeanStatus.State = CameraLeanState.Off;
            } else if (command.LeanLeft) {
                playerCamera.LeanStatus.State = CameraLeanState.Left;
            } else if (command.LeanRight) {
                playerCamera.LeanStatus.State = CameraLeanState.Right;
            }

            // Camera rotates around the target by the same amount as the target rotation.
            var rotationDiff = command.TargetRotation * Quaternion.Inverse(rotation.Value);
            var localPosition = Quaternion.Inverse(command.TargetRotation) * (translation.Value - (float3)command.TargetPosition);
            var updatedLocalPosition = rotationDiff * localPosition;
            translation.Value = command.TargetRotation * updatedLocalPosition + command.TargetPosition;
            rotation.Value = command.TargetRotation;

            if (playerCamera.LeanStatus.State == CameraLeanState.Off) {
                var destination = CalculateWallAvoidingTranslation(command.TargetRotation * cameraOffset + command.TargetPosition, command.TargetPosition);
                translation.Value.x = Mathf.Lerp(translation.Value.x, destination.x, 5f * this.Time.DeltaTime);
                translation.Value.y = Mathf.Lerp(translation.Value.y, destination.y, 10f * this.Time.DeltaTime);
                translation.Value.z = Mathf.Lerp(translation.Value.z, destination.z, 10f * this.Time.DeltaTime);
            } else {
                var leanDirection = playerCamera.LeanStatus.State == CameraLeanState.Left ? Vector3.left : Vector3.right;
                var leanOffset = playerCamera.leanLength * leanDirection;
                playerCamera.LeanStatus.currentLeanOffset = Vector3.Lerp(playerCamera.LeanStatus.currentLeanOffset, leanOffset, 30f * this.Time.DeltaTime);
                var localTranslation = cameraOffset + playerCamera.LeanStatus.currentLeanOffset;
                translation.Value = command.TargetRotation * localTranslation + command.TargetPosition;
            }
            // Double check wall penetration to avoid jittering.
            translation.Value = CalculateWallAvoidingTranslation(translation.Value, command.TargetPosition);
        });
    }

    private Vector3 CalculateWallAvoidingTranslation(Vector3 cameraPosition, Vector3 targetPosition) {
        var positionFromTarget = cameraPosition - targetPosition;
        var layerMask = ~(1 << LayerMask.NameToLayer("Player"));
        var isRayHit =
            Physics.Raycast(targetPosition, positionFromTarget,
            out RaycastHit hit, positionFromTarget.magnitude + 0.04f,
            layerMask, QueryTriggerInteraction.Ignore);
        if (isRayHit) {
            Debug.Log($"{hit.distance}, {hit.collider.gameObject.name}");
        }
        return isRayHit
             ? hit.point - positionFromTarget.normalized * 0.03f
             : cameraPosition;
    }

    //private void SetLeanType() {
    //    if (!this.IsTargetMoving) {
    //        if (this.IsRightLeanTriggered) {
    //            TriggerLean(CameraLeanMode.Right);
    //        }
    //        if (this.IsLeftLeanTriggered) {
    //            TriggerLean(CameraLeanMode.Left);
    //        }
    //    }
    //    var isLeanCanceled =
    //        this.currentLeanMode == CameraLeanMode.Left && !Input.GetButton("right lean") ||
    //        this.currentLeanMode == CameraLeanMode.Right && !Input.GetButton("left lean");
    //    if (isLeanCanceled) {
    //        TriggerLean(CameraLeanMode.None);
    //    }
    //}

    //private void TriggerLean(CameraLeanMode leanType) {
    //    switch (leanType) {
    //        case CameraLeanMode.Left:
    //            Game.SoundSystem.Play(SoundDefinition.LeanSound);
    //            this.currentLeanMode = CameraLeanMode.Left;
    //            break;
    //        case CameraLeanMode.Right:
    //            Game.SoundSystem.Play(SoundDefinition.LeanSound);
    //            this.currentLeanMode = CameraLeanMode.Right;
    //            break;
    //        case CameraLeanMode.None:
    //            this.currentLeanMode = CameraLeanMode.None;
    //            break;
    //    }
    //    this.currentLeanOffset = Vector3.zero;
    //}
}

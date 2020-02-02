using Unity.Entities;
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
    public Entity Target;
    public float leanLength;// = 5f;
    public float offsetDistanceByPitch;// = 4.7f;
    public bool IsRightLeanTriggered => Input.GetButtonDown("right lean");
    public bool IsLeftLeanTriggered => Input.GetButtonDown("left lean");
}


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
            var offsetLength = Mathf.Abs(Mathf.Sin(pitch)) * playerCamera.offsetDistanceByPitch;
            var cameraOffset = playerCamera.BaseCameraOffset + offsetLength * Vector3.forward;
            var targetTransform = this.EntityManager.GetComponentObject<Transform>(playerCamera.Target);
            if (command.LeanLeft) {
                //            camera.Lea
            }
            //SetLeanType();

            // Camera rotates around the target by the same amount as the target rotation.
            var rotationDiff = targetTransform.rotation * Quaternion.Inverse(rotation.Value);
            var localPosition = targetTransform.InverseTransformPoint(translation.Value);
            var updatedLocalPosition = rotationDiff * localPosition;
            translation.Value = targetTransform.TransformPoint(updatedLocalPosition);
            rotation.Value = targetTransform.rotation;

            if (playerCamera.LeanStatus.State == CameraLeanState.Off) {
                var destination = CalculateWallAvoidingTranslation(targetTransform.TransformPoint(cameraOffset), targetTransform.position);
                translation.Value.x = Mathf.Lerp(translation.Value.x, destination.x, 7f * this.Time.DeltaTime);
                translation.Value.y = Mathf.Lerp(translation.Value.y, destination.y, 20f * this.Time.DeltaTime);
                translation.Value.z = Mathf.Lerp(translation.Value.z, destination.z, 20f * this.Time.DeltaTime);
            } else {
                var leanDirection = playerCamera.LeanStatus.State == CameraLeanState.Left ? Vector3.left : Vector3.right;
                var leanOffset = playerCamera.leanLength * leanDirection;
                playerCamera.LeanStatus.currentLeanOffset = Vector3.Lerp(playerCamera.LeanStatus.currentLeanOffset, leanOffset, 30f * this.Time.DeltaTime);
                translation.Value = targetTransform.TransformPoint(cameraOffset + playerCamera.LeanStatus.currentLeanOffset);
            }
            // Double check wall penetration to avoid jittering.
            translation.Value = CalculateWallAvoidingTranslation(translation.Value, targetTransform.position);
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

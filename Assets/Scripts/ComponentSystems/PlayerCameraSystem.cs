using Unity.Entities;
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
        this.query = GetEntityQuery(typeof(PlayerCamera), typeof(PlayerCameraCommand), typeof(Transform));
    }

    private Vector3 CalculateOffset(PlayerCamera camera, PlayerCameraCommand command) {
        var offsetLength = Mathf.Abs(Mathf.Sin(command.TargetRotation.eulerAngles.x * Mathf.Deg2Rad)) * camera.offsetDistanceByPitch;
        return offsetLength * Vector3.forward;
    }

    protected override void OnUpdate() {
        var camera = this.query.GetSingleton<PlayerCamera>();
        var command = this.query.GetSingleton<PlayerCameraCommand>();
        var transform = this.EntityManager.GetComponentObject<Transform>(this.query.GetSingletonEntity());
        var cameraOffset = camera.BaseCameraOffset + CalculateOffset(camera, command);
        var targetTransform = this.EntityManager.GetComponentObject<Transform>(camera.Target);
        if (command.LeanLeft) {
//            camera.Lea
        }
        //SetLeanType();

        UpdateByTargetRotation(transform, targetTransform);
        if (camera.LeanStatus.State == CameraLeanState.Off) {
            var desiredCameraPosition = AvoidWallPenetration(targetTransform.TransformPoint(cameraOffset), targetTransform.position);
            Follow(transform, desiredCameraPosition);
        } else {
            var leanDirection = camera.LeanStatus.State == CameraLeanState.Left ? Vector3.left : Vector3.right;
            var leanOffset = camera.leanLength * leanDirection;
            camera.LeanStatus.currentLeanOffset = Vector3.Lerp(camera.LeanStatus.currentLeanOffset, leanOffset, 30f * this.Time.DeltaTime);
            transform.position = targetTransform.TransformPoint(cameraOffset + camera.LeanStatus.currentLeanOffset);
        }
        // Double check wall penetration to avoid jittering.
        transform.position = AvoidWallPenetration(transform.position, targetTransform.position);
    }

    // Converts position for avoiding penetration into walls.
    private Vector3 AvoidWallPenetration(Vector3 cameraPosition, Vector3 targetPosition) {
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

    private void UpdateByTargetRotation(Transform camera, Transform target) {
        // Camera rotates around the target by the same amount as rotation of the target from the last frame.
        var rotationDiff = target.transform.rotation * Quaternion.Inverse(camera.transform.rotation);
        var localPosition = target.InverseTransformPoint(camera.transform.position);
        var updatedLocalPosition = rotationDiff * localPosition;
        camera.transform.position = target.TransformPoint(updatedLocalPosition);
        camera.transform.rotation = target.rotation;
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

    private void Follow(Transform camera, Vector3 destination) {
        var cameraPosition = camera.transform.position;
        cameraPosition.x = Mathf.Lerp(cameraPosition.x, destination.x, 7f * this.Time.DeltaTime);
        cameraPosition.y = Mathf.Lerp(cameraPosition.y, destination.y, 20f * this.Time.DeltaTime);
        cameraPosition.z = Mathf.Lerp(cameraPosition.z, destination.z, 20f * this.Time.DeltaTime);
        camera.position = cameraPosition;
    }
}

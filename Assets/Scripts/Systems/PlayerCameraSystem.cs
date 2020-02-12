using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public enum CameraFollowMode {
    Smooth,
    Lazy,
    LeanRight,
    LeanLeft
}

public struct PlayerCameraCommand : IComponentData {
    public CameraFollowMode RequestedMode;
    public float MaxSpeed;
    public float MaxDistance;
    public Entity TargetEntity;
}

public struct CameraLeanStatus {
    public Vector3 currentLeanOffset;
}

public struct PlayerCamera : IComponentData {
    public CameraLeanStatus LeanStatus;
    public CameraFollowMode FollowMode;
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
        var cameraEntity = this.query.GetSingletonEntity();
        var camera = this.EntityManager.GetComponentData<PlayerCamera>(cameraEntity);
        var command = this.EntityManager.GetComponentData<PlayerCameraCommand>(cameraEntity);
        var translation = this.EntityManager.GetComponentData<Translation>(cameraEntity);
        var rotation = this.EntityManager.GetComponentData<Rotation>(cameraEntity);
        var targetRotation = this.EntityManager.GetComponentData<Rotation>(command.TargetEntity);
        var targetTranslation = this.EntityManager.GetComponentData<Translation>(command.TargetEntity);
        var pitch = ((Quaternion)targetRotation.Value).eulerAngles.x * Mathf.Deg2Rad;
        var offsetLength = Mathf.Abs(Mathf.Sin(pitch)) * camera.forwardOffsetFactor;
        var cameraOffset = camera.BaseCameraOffset + offsetLength * Vector3.forward;

        //if (command.RequestedMode != CameraFollowMode.None) {
        //    playerCamera.Mode = command.RequestedMode;
        //}

        // Camera rotates around the target by the same amount as the target rotation.
        var rotationDiff = targetRotation.Value * Quaternion.Inverse(rotation.Value);
        var localPosition = Quaternion.Inverse(targetRotation.Value) * (translation.Value - targetTranslation.Value);
        var updatedLocalPosition = rotationDiff * localPosition;
        translation.Value = (float3)((Quaternion)targetRotation.Value * updatedLocalPosition) + targetTranslation.Value;
        rotation.Value = targetRotation.Value;
        var defaultPosition = (Quaternion)targetRotation.Value * cameraOffset + (Vector3)targetTranslation.Value;

        //var wallAvoidedTranslation = AvoidObstacles(defaultPosition, command.TargetPosition);
        switch (camera.FollowMode) {
            //case CameraMode.Sticked:
            //    translation.Value.x = Mathf.Lerp(translation.Value.x, destination.x, 5f * this.Time.DeltaTime);
            //    translation.Value.y = Mathf.Lerp(translation.Value.y, destination.y, 10f * this.Time.DeltaTime);
            //    translation.Value.z = Mathf.Lerp(translation.Value.z, destination.z, 10f * this.Time.DeltaTime);
            case CameraFollowMode.Smooth:
                var currentOffset = (Vector3)translation.Value - defaultPosition;
                if ((currentOffset - currentOffset.normalized * command.MaxSpeed * this.Time.DeltaTime).magnitude > command.MaxDistance) {
                    translation.Value = defaultPosition + currentOffset.normalized * command.MaxDistance;
                } else if (currentOffset.magnitude < command.MaxSpeed * this.Time.DeltaTime) {
                    translation.Value = defaultPosition;
                } else {
                    translation.Value -= (float3)currentOffset.normalized * command.MaxSpeed * this.Time.DeltaTime;
                }
                break;
            case CameraFollowMode.LeanLeft:
            case CameraFollowMode.LeanRight:
                var leanDirection = camera.FollowMode == CameraFollowMode.LeanLeft
                    ? Vector3.left : Vector3.right;
                var leanOffset = playerCamera.leanLength * leanDirection;
                playerCamera.LeanStatus.currentLeanOffset = Vector3.Lerp(playerCamera.LeanStatus.currentLeanOffset, leanOffset, 30f * this.Time.DeltaTime);
                var localTranslation = cameraOffset + playerCamera.LeanStatus.currentLeanOffset;
                translation.Value = command.TargetRotation * localTranslation + command.TargetPosition;
                break;
        }
        // Double check wall penetration to avoid jittering.
        translation.Value = AvoidObstacles(translation.Value, command.TargetPosition);
    });
    }

private Vector3 AvoidObstacles(Vector3 cameraPosition, Vector3 targetPosition) {
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

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using static Unity.Mathematics.math;

public enum CameraFollowMode {
    Smooth,
    Clamp,
    LeanRight,
    LeanLeft
}

public struct PlayerCameraCommand : IComponentData {
    public CameraFollowMode RequestedMode;
    public float MaxSpeed;
    public float3 MaxOffset;
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
    public float3 AimingPoint;
}

[UpdateAfter(typeof(MechMovementUpdateSystem)), UpdateAfter(typeof(PlayerInputSystem))]
public class PlayerCameraSystem : ComponentSystem {
    private EntityQuery query;

    protected override void OnCreate() {
        this.query = GetEntityQuery(
            typeof(PlayerCamera), typeof(PlayerCameraCommand), typeof(Translation),
            typeof(Rotation));
    }

    protected override void OnUpdate() {
        if (this.query.CalculateEntityCount() == 0) {
            return;
        }
        var cameraEntity = this.query.GetSingletonEntity();
        var camera = this.EntityManager.GetComponentData<PlayerCamera>(cameraEntity);
        var command = this.EntityManager.GetComponentData<PlayerCameraCommand>(cameraEntity);
        if (!this.EntityManager.Exists(command.TargetEntity)) {
            return;
        }
        var translation = this.EntityManager.GetComponentData<Translation>(cameraEntity);
        var rotation = this.EntityManager.GetComponentData<Rotation>(cameraEntity);
        //var targetRotation = this.EntityManager.GetComponentData<Rotation>(command.TargetEntity);
        //var targetTranslation = this.EntityManager.GetComponentData<Translation>(command.TargetEntity);
        var transform = this.EntityManager.GetComponentObject<CharacterController>(command.TargetEntity).transform;
        var targetRotation = new Rotation { Value = transform.rotation };
        var targetTranslation = new Translation { Value = (float3)transform.position + this.EntityManager.GetComponentData<Mech>(command.TargetEntity).BaseCameraOffset };
        var pitch = ((Quaternion)targetRotation.Value).eulerAngles.x * Mathf.Deg2Rad;
        var yaw = ((Quaternion)targetRotation.Value).eulerAngles.y * Mathf.Deg2Rad;
        var offsetLength = Mathf.Abs(Mathf.Sin(pitch)) * camera.forwardOffsetFactor;
        var cameraOffset = camera.BaseCameraOffset + offsetLength * Vector3.forward;
        command.MaxOffset = new float3(1.5f, 0.4f, 0f);
        // Camera rotates around the target by the same amount as the target rotation.
        var rotationDiff = targetRotation.Value * Quaternion.Inverse(rotation.Value);
        var localPosition = Quaternion.Inverse(targetRotation.Value) * (translation.Value - targetTranslation.Value);
        var updatedLocalPosition = rotationDiff * localPosition;
        translation.Value = mul(targetRotation.Value, updatedLocalPosition) + targetTranslation.Value;
        rotation.Value = targetRotation.Value;
        // Camera follow
        var defaultPosition = mul(targetRotation.Value, cameraOffset) + targetTranslation.Value;
        var targetMovement = this.EntityManager.GetComponentData<MechMovementStatus>(command.TargetEntity);
        camera.FollowMode = targetMovement.State == MechMovementState.Stand || targetMovement.State == MechMovementState.BoostBraking || targetMovement.State == MechMovementState.BoostAcceling
            ? CameraFollowMode.Smooth : CameraFollowMode.Clamp;
        var localOffset = mul(Unity.Mathematics.quaternion.AxisAngle(float3(0f, 1f, 0f), -yaw), translation.Value - defaultPosition);
        var targetLocalMotion = mul(inverse(targetRotation.Value), targetMovement.Velocity) * this.Time.DeltaTime;
        switch (camera.FollowMode) {
            case CameraFollowMode.Smooth: {
                    localOffset += targetLocalMotion;
                    if (localOffset.Equals(float3(0f, 0f, 0f))) {
                        break;
                    }
                    var lerpSpeed = targetMovement.State == MechMovementState.BoostBraking || targetMovement.State == MechMovementState.BoostAcceling
                        ? 10f : 6f;
                    var motion = -normalize(localOffset) * lerpSpeed * this.Time.DeltaTime;
                    if (lengthsq(motion) > lengthsq(localOffset)) {
                        localOffset = float3(0f, 0f, 0f);
                    } else {
                        localOffset += motion;
                    }
                    break;
                }
            case CameraFollowMode.Clamp:
                localOffset += targetLocalMotion * 0.3f;
                localOffset = clamp(localOffset, -command.MaxOffset, command.MaxOffset);
                break;
            case CameraFollowMode.LeanLeft:
            case CameraFollowMode.LeanRight:
                var leanDirection = camera.FollowMode == CameraFollowMode.LeanLeft
                    ? Vector3.left : Vector3.right;
                var leanOffset = camera.leanLength * leanDirection;
                camera.LeanStatus.currentLeanOffset = Vector3.Lerp(camera.LeanStatus.currentLeanOffset, leanOffset, 30f * this.Time.DeltaTime);
                localOffset = cameraOffset + camera.LeanStatus.currentLeanOffset;
                break;
        }
        translation.Value = mul(Unity.Mathematics.quaternion.AxisAngle(float3(0f, 1f, 0f), yaw), localOffset) + defaultPosition;
        translation.Value = AvoidObstacles(translation.Value, targetTranslation.Value);

        Assert.IsFalse(float.IsNaN(translation.Value.x) || float.IsNaN(translation.Value.y) || float.IsNaN(translation.Value.z));

        this.EntityManager.SetComponentData(cameraEntity, translation);
        this.EntityManager.SetComponentData(cameraEntity, rotation);
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

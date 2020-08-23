using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;

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
    public float LeanLength;// = 5f;
    public float ForwardOffsetFactor;// = 4.7f;
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
        var targetTranslation = this.EntityManager.GetComponentData<Translation>(command.TargetEntity).Value;
        targetTranslation += this.EntityManager.GetComponentData<Mech>(command.TargetEntity).CameraOffset;
        var targetMovement = this.EntityManager.GetComponentData<MechMovementStatus>(command.TargetEntity);
        var pitch = targetMovement.Pitch;
        var yaw = targetMovement.Yaw;
        var targetRotation = quaternion.Euler(new float3(pitch, yaw, 0f));
        var forwardOffsetLength = math.abs(math.sin(pitch)) * camera.ForwardOffsetFactor - 5f;
        var cameraOffsetByTargetPitch = forwardOffsetLength * math.forward(quaternion.identity);

        // TODO: Move this to better place
        command.MaxOffset = new float3(1.5f, 0.4f, 0f);
        camera.FollowMode = targetMovement.State == MechMovementState.Stand || targetMovement.State == MechMovementState.BoostBraking || targetMovement.State == MechMovementState.BoostAcceling
            ? CameraFollowMode.Smooth : CameraFollowMode.Clamp;

        // 1. Camera rotationtarget rotation.
        var rotationDiff = targetRotation * Quaternion.Inverse(rotation.Value);
        var localPosition = Quaternion.Inverse(targetRotation) * (translation.Value - targetTranslation);
        var updatedLocalPosition = rotationDiff * localPosition;
        translation.Value = math.mul(targetRotation, updatedLocalPosition) + targetTranslation;
        rotation.Value = targetRotation;

        // 2. Move camera position.
        var defaultPosition = math.mul(targetRotation, cameraOffsetByTargetPitch) + targetTranslation;
        var localOffset = math.mul(quaternion.AxisAngle(new float3(0f, 1f, 0f), -yaw), translation.Value - defaultPosition);
        var targetLocalMotion = math.mul(math.inverse(targetRotation), targetMovement.Velocity) * this.Time.DeltaTime;
        switch (camera.FollowMode) {
            case CameraFollowMode.Smooth: {
                    localOffset += targetLocalMotion;
                    if (localOffset.Equals(new float3(0f, 0f, 0f))) {
                        break;
                    }
                    var lerpSpeed = targetMovement.State == MechMovementState.BoostBraking || targetMovement.State == MechMovementState.BoostAcceling
                        ? 10f : 6f;
                    var motion = -math.normalize(localOffset) * lerpSpeed * this.Time.DeltaTime;
                    if (math.lengthsq(motion) > math.lengthsq(localOffset)) {
                        localOffset = new float3(0f, 0f, 0f);
                    } else {
                        localOffset += motion;
                    }
                    break;
                }
            case CameraFollowMode.Clamp:
                localOffset += targetLocalMotion * 0.3f;
                localOffset = math.clamp(localOffset, -command.MaxOffset, command.MaxOffset);
                break;
            case CameraFollowMode.LeanLeft:
            case CameraFollowMode.LeanRight:
                var leanDirection = camera.FollowMode == CameraFollowMode.LeanLeft
                    ? Vector3.left : Vector3.right;
                var leanOffset = camera.LeanLength * leanDirection;
                camera.LeanStatus.currentLeanOffset = Vector3.Lerp(camera.LeanStatus.currentLeanOffset, leanOffset, 30f * this.Time.DeltaTime);
                localOffset = cameraOffsetByTargetPitch + (float3)camera.LeanStatus.currentLeanOffset;
                break;
        }
        translation.Value = math.mul(quaternion.AxisAngle(math.float3(0f, 1f, 0f), yaw), localOffset) + defaultPosition;

        // 3. Wall penetration check
        translation.Value = AvoidObstacles(translation.Value, targetTranslation);

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

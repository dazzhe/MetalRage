using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MechMovementRequestSystem))]
[UpdateBefore(typeof(CharacterControllerFollowGroundSystem))]
public class MechCharacterPhysicsInputSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((
            ref Translation translation,
            ref MechRequestedMovement movement,
            ref CharacterPhysicsInput physicsInput,
            ref CharacterPhysicsVelocity velocity,
            ref MechMovementConfigData config
        ) => {
            physicsInput.FollowGround = movement.ShouldFollowGround;
            physicsInput.StartPosition = translation.Value;
            physicsInput.CheckSupport = true;
            velocity.Velocity = movement.Velocity;
            if (movement.ShouldApplyGravity) {
                velocity.Velocity += math.up() * config.Gravity * this.Time.DeltaTime;
            }
        });
    }
}


[UpdateAfter(typeof(CharacterControllerStepSystem))]
public class MechMovementUpdateSystem : ComponentSystem {
    protected override void OnUpdate() {
        var dt = this.Time.DeltaTime;
        this.Entities.ForEach((
            ref Translation translation,
            ref CharacterPhysicsOutput physicsOutput,
            ref CharacterPhysicsVelocity physicsVelocity,
            ref GroundContactStatus groundContactStatus,
            ref MechRequestedMovement requestedMovement,
            ref MechMovementStatus status
        ) => {
            translation.Value = physicsOutput.MoveResult;
            status.Velocity = physicsVelocity.Velocity;
            status.LegYaw = Mathf.Lerp(status.LegYaw, requestedMovement.LegYaw, 10f * dt);
            status.IsOnGround =
                groundContactStatus.SupportedState == CharacterControllerUtilities.CharacterSupportState.Supported
             && requestedMovement.ShouldFollowGround;
            if (status.IsOnGround && requestedMovement.State == MechMovementState.Airborne) {
                status.State = MechMovementState.Stand;
            } else if (!status.IsOnGround && requestedMovement.State != MechMovementState.BoostAcceling) {
                status.State = MechMovementState.Airborne;
            } else {
                status.State = requestedMovement.State;
            }
        });
    }
}

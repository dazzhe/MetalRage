using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MechMovementRequestSystem))]
[UpdateBefore(typeof(CharacterControllerSystem))]
public class MechCharacterPhysicsInputSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((
            ref Translation translation,
            ref MechRequestedMovement movement,
            ref CharacterControllerInput physicsInput,
            ref CharacterControllerInternalData ccInternalData,
            ref MechMovementConfigData config
        ) => {
            physicsInput.Jumped = movement.ShouldJump ? 1 : 0;
            physicsInput.Movement = new float2 {
                x = movement.Velocity.x,
                y = movement.Velocity.z
            };
        });
    }
}


[UpdateAfter(typeof(CharacterControllerSystem))]
public class MechMovementUpdateSystem : ComponentSystem {
    protected override void OnUpdate() {
        var dt = this.Time.DeltaTime;
        this.Entities.ForEach((
            ref CharacterControllerInternalData ccInternalData,
            ref MechRequestedMovement requestedMovement,
            ref MechMovementStatus status
        ) => {
            status.IsOnGround = ccInternalData.SupportedState == CharacterControllerUtilities.CharacterSupportState.Supported;
            status.Velocity = ccInternalData.LinearVelocity;
            status.LegYaw = Mathf.Lerp(status.LegYaw, requestedMovement.LegYaw, 10f * dt);
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

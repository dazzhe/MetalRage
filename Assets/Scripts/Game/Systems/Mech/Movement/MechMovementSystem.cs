using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct MechRequestedMovement : IComponentData {
    public float3 Motion;
    public MechMovementState State;
    public float LegYaw;
    public Bool UseRawMotion;
}

// TODO : Boost while crouching
// TODO : Multiple input at the same frame
[UpdateAfter(typeof(MechCommandSystem))]
public class MechMovementRequestSystem : ComponentSystem {
    private static BoostAction boostAction = new BoostAction();
    private static BoostStateBehaviour boostStateBehaviour = new BoostStateBehaviour();
    private static MechMovementAction[] actions = new MechMovementAction[] {
        new CrouchAction(),
        new JumpAction(),
    };
    private static Dictionary<MechMovementState, MechStateBehaviour> stateBehaviours = new Dictionary<MechMovementState, MechStateBehaviour> {
        { MechMovementState.Stand, new WalkStateBehaviour() },
        { MechMovementState.Walking, new WalkStateBehaviour() },
        { MechMovementState.Crouching, new WalkStateBehaviour() },
        { MechMovementState.Airborne, new AirborneStateBehaviour() },
    };

    protected override void OnUpdate() {
        this.Entities.ForEach((
            ref MechCommand command,
            ref MechMovementStatus status,
            ref MechRequestedMovement requestedMovement,
            ref MechMovementConfigData config,
            ref BoosterConfigData boostConfig,
            ref BoosterEngineStatus engineStatus) => {
                foreach (var action in actions) {
                    action.Initialize(command, status, config);
                }
                //boostAction.Initialize(command, this.EntityManager.GetComponentObject<MechComponent>(entity).BoosterEffect);
                var hasExecutedAction = false;
                if (boostAction.IsExecutable(status, config, boostConfig, engineStatus)) {
                    requestedMovement = boostAction.CalculateMovement(status, config, boostConfig, ref engineStatus);
                    hasExecutedAction = true;
                } else {
                    foreach (var action in actions) {
                        if (action.IsExecutable()) {
                            requestedMovement = action.CalculateMovement();
                            hasExecutedAction = true;
                            break;
                        }
                    }
                }
                if (!hasExecutedAction) {
                    switch (status.State) {
                        case MechMovementState.BoostAcceling:
                        case MechMovementState.BoostBraking:
                            requestedMovement = boostStateBehaviour.ComputeMovement(status, config, boostConfig, ref engineStatus);
                            break;
                        default:
                            requestedMovement = stateBehaviours[status.State].ComputeMovement(command, status, config);
                            break;
                    }
                }
            });
    }
}

[UpdateAfter(typeof(MechMovementRequestSystem))]
public class MechMovementUpdateSystem : ComponentSystem {
    protected override void OnUpdate() {
        var dt = this.Time.DeltaTime;
        this.Entities.ForEach((CharacterController characterController, ref MechRequestedMovement requestedMovement, ref MechCommand command, ref MechMovementStatus status, ref MechMovementConfigData config) => {
            status.Yaw = characterController.transform.eulerAngles.y + command.DeltaLook.x * Preferences.Sensitivity.GetFloat();
            characterController.transform.eulerAngles = new Vector3(0f, status.Yaw, 0f);
            status.Yaw *= Mathf.Deg2Rad;
            status.Pitch -= command.DeltaLook.y * Preferences.Sensitivity.GetFloat();
            status.Pitch = Mathf.Clamp(status.Pitch, config.MinPitch, config.MaxPitch);

            var prevPosition = characterController.transform.position;
            var motion = requestedMovement.Motion;
            if (requestedMovement.UseRawMotion) {
                characterController.Move(motion);
            } else {
                // Use gravity instead of requestedMovement.motion.y.
                if (status.IsOnGround) {
                    var testMotion = motion;
                    var horizontalMotion = new Vector3(requestedMovement.Motion.x, 0, requestedMovement.Motion.z).magnitude;
                    testMotion.y = -horizontalMotion * math.tan(math.radians(config.MaxSlopeAngle)) +
                        0.5f * config.Gravity * dt * dt;
                    characterController.Move(testMotion);
                    if (!characterController.isGrounded) {
                        // Revert motion and prepare for airborne mortion.
                        characterController.Move(-testMotion);
                        status.IsOnGround = false;
                        status.Velocity.y = 0f;
                    }
                }
                if (!status.IsOnGround) {
                    motion.y = status.Velocity.y * dt +
                        0.5f * config.Gravity * dt * dt;
                    motion.y = math.clamp(motion.y, -config.MaxFallSpeed, config.MaxFallSpeed);
                    characterController.Move(motion);
                }
            }
            status.Velocity = (characterController.transform.position - prevPosition) / dt;
            status.LegYaw = Mathf.Lerp(status.LegYaw, requestedMovement.LegYaw, 10f * dt);
            status.IsOnGround = characterController.isGrounded;
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

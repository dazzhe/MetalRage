using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct MechMovementConfigData : IComponentData {
    [Header("Walk")]
    public float WalkAccel;
    public float Decel;
    public float MaxWalkSpeed;
    [Header("Jump")]
    public float BaseJumpSpeed;
}

public class MechMovementSystem : ComponentSystem {
    private static BoostAction boostAction = new BoostAction();
    private static MechBoostStateBehaviour boostStateBehaviour = new MechBoostStateBehaviour();
    private static MechMovementAction[] actions = new MechMovementAction[] {
        new CrouchAction(),
    };
    private static Dictionary<MechMovementState, MechStateBehaviour> stateBehaviours = new Dictionary<MechMovementState, MechStateBehaviour> {
        { MechMovementState.Idle, new WalkStateBehaviour() },
        { MechMovementState.Walking, new WalkStateBehaviour() },
        { MechMovementState.Crouching, new WalkStateBehaviour() }
    };

    protected override void OnUpdate() {
        this.Entities.ForEach((CharacterController characterController, ref MechMovementStatus status, ref MechMovementConfigData config, ref BoostActionConfigData boostConfig, ref BoosterEngineStatus engineStatus) => {
            MechMovementAction activatableAction = null;
            MechRequestedMovement requestedMovement;
            if (boostAction.IsActivatable(status, config, boostConfig, engineStatus)) {
                requestedMovement = boostAction.ComputeMovement(status, config, boostConfig, ref engineStatus);
            } else {
                foreach (var action in actions) {
                    if (action.IsActivatable(status, config)) {
                        activatableAction = action;
                        break;
                    }
                }
                if (activatableAction != null) {
                    requestedMovement = activatableAction.ComputeMovement(status, config);
                } else {
                    switch (status.State) {
                        case MechMovementState.Acceling:
                        case MechMovementState.Boosting:
                        case MechMovementState.Braking:
                            requestedMovement = boostStateBehaviour.ComputeMovement(status, config, boostConfig, ref engineStatus);
                            break;
                        default:
                            requestedMovement = stateBehaviours[status.State].ComputeMovement(status, config);
                            break;
                    }
                }
            }
            var prevPosition = characterController.transform.position;
            characterController.Move(requestedMovement.Motion);
            status.Velocity = Quaternion.Inverse(characterController.transform.rotation) * (characterController.transform.position - prevPosition) / Time.DeltaTime;
            status.State = requestedMovement.State;
            status.LegYaw = Mathf.Lerp(status.LegYaw, requestedMovement.LegYaw, 10f * Time.DeltaTime);
            status.IsOnGround = requestedMovement.IsLeavingGround ? true : characterController.isGrounded;
        });
    }
}

public abstract class MechStateBehaviour {
    public abstract MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config);
}

public class WalkStateBehaviour : MechStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config) {
        var motion = new Vector3(status.Velocity.x, 0f, status.Velocity.z);
        motion.x += InputSystem.GetMoveHorizontal() * config.WalkAccel * Time.deltaTime;
        motion.z += InputSystem.GetMoveVertical() * config.WalkAccel * Time.deltaTime;
        if (InputSystem.GetMoveHorizontal() == 0) {
            var xAbs = Mathf.Max(Mathf.Abs(motion.x) - config.Decel * Time.deltaTime, 0f);
            motion.x = motion.x > 0 ? xAbs : -xAbs;
        }
        if (InputSystem.GetMoveVertical() == 0) {
            var zAbs = Mathf.Max(Mathf.Abs(motion.z) - config.Decel * Time.deltaTime, 0f);
            motion.z = motion.z > 0 ? zAbs : -zAbs;
        }
        motion = motion.magnitude > config.MaxWalkSpeed ? motion.normalized * config.MaxWalkSpeed : motion;
        motion.y = -900f;
        var movement = new MechRequestedMovement {
            Motion = motion * Time.deltaTime,
            State = motion.magnitude == 0 ? MechMovementState.Idle : MechMovementState.Walking
        };
        var x = InputSystem.GetMoveHorizontal();
        var z = InputSystem.GetMoveVertical();
        movement.LegYaw = x > 0 && z > 0 ? 45f :
                      x > 0 && z == 0 ? 90f :
           (x == 0 && z > 0) || z < 0 ? 0f :
                       x < 0 && z > 0 ? -45f :
                      x < 0 && z == 0 ? -90f : status.LegYaw;
        return movement;
    }
}

public class MechAirborne : MechStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config) {
        var movement = new MechRequestedMovement();
        movement.State = MechMovementState.Airborne;
        movement.Motion.x += InputSystem.GetMoveHorizontal() * 40f * Time.deltaTime;
        movement.Motion.y -= 9.8f * Time.deltaTime;
        movement.Motion.z += InputSystem.GetMoveVertical() * 40f * Time.deltaTime;
        return movement;
    }
}

public class MechBoostStateBehaviour {
    public MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config, BoostActionConfigData boostConfig, ref BoosterEngineStatus engineStatus) {
        engineStatus.ElapsedTime += Time.deltaTime;
        var movement = new MechRequestedMovement();
        switch (status.State) {
            case MechMovementState.Acceling:
                movement.Motion = status.Velocity * Time.deltaTime;
                movement.State = engineStatus.ElapsedTime > 0.05f
                    ? MechMovementState.Boosting
                    : MechMovementState.Acceling;
                break;
            case MechMovementState.Boosting:
                movement.Motion = status.Velocity * Time.deltaTime;
                movement.State = engineStatus.ElapsedTime > boostConfig.Duration
                    ? MechMovementState.Braking
                    : MechMovementState.Boosting;
                break;
            case MechMovementState.Braking:
                movement.Motion = Vector3.Lerp(status.Velocity, Vector3.zero, 6f * Time.deltaTime) * Time.deltaTime;
                if (status.Velocity.magnitude <= config.MaxWalkSpeed) {
                    movement.State = MechMovementState.Walking;
                }
                break;
        }
        return movement;
    }
}

public abstract class MechMovementAction {
    public abstract MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config);
    public abstract bool IsActivatable(MechMovementStatus status, MechMovementConfigData config);
}

public class CrouchAction : MechMovementAction {
    public override MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config) {
        var command = new MechRequestedMovement {
            State = MechMovementState.Crouching,
            Motion = Vector3.zero,
            LegYaw = status.LegYaw
        };
        return command;
    }

    public override bool IsActivatable(MechMovementStatus status, MechMovementConfigData config) {
        var isRequested = InputSystem.GetButton(MechCommandButton.Crouch);
        var state = status.State;
        var isAllowed =
            state == MechMovementState.Acceling || state == MechMovementState.Braking ||
            state == MechMovementState.Crouching || state == MechMovementState.Idle ||
            state == MechMovementState.Walking;
        return isRequested && isAllowed;
    }
}

public class BoostAction {
    public MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config, BoostActionConfigData boostConfig, ref BoosterEngineStatus engineStatus) {
        engineStatus.Gauge += 2f;
        var movement = new MechRequestedMovement();
        engineStatus.Gauge -= 28;
        engineStatus.ElapsedTime = 0f;
        var inertiaDirection = status.Velocity.normalized;
        var inputDirection = new Vector3(InputSystem.GetMoveHorizontal(), 0f, InputSystem.GetMoveVertical());
        Vector3 boostDirection;
        if (inputDirection.z < 0 || inputDirection.magnitude == 0) {
            // Mech cannot boost backward.
            boostDirection = Vector3.forward;
        } else if (inputDirection.z > 0) {
            boostDirection = inputDirection;
        } else if (Vector3.Dot(inputDirection, inertiaDirection) < 0f) {
            boostDirection = inputDirection;
        } else {
            boostDirection = inertiaDirection;
        }
        movement.LegYaw = 0f;
        movement.Motion = boostDirection * boostConfig.MaxSpeed * Time.deltaTime;
        movement.State = MechMovementState.Acceling;
        return movement;
    }

    public bool IsActivatable(MechMovementStatus status, MechMovementConfigData config, BoostActionConfigData boostConfig, BoosterEngineStatus engineStatus) {
        var state = status.State;
        var isRequested = InputSystem.GetButtonDown(MechCommandButton.Boost);
        var hasEnoughGauge = engineStatus.Gauge >= boostConfig.Consumption;
        var isAllowed = state != MechMovementState.Airborne;
        return isRequested && hasEnoughGauge && isAllowed;
    }
}

public class JumpAction : MechMovementAction {
    public override MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config) {
        var command = new MechRequestedMovement();
        command.State = MechMovementState.Airborne;
        command.IsLeavingGround = true;
        //this.engine?.ShowJetFlame(0.4f, Vector3.forward);
        // Jumping power is proportial to current moving speed.
        command.Motion = new Vector3 {
            x = status.Velocity.x,
            y = config.BaseJumpSpeed * (1f + 0.002f * status.Velocity.magnitude),
            z = status.Velocity.z
        } * Time.deltaTime;
        return command;
    }

    public override bool IsActivatable(MechMovementStatus status, MechMovementConfigData config) {
        var isRequested = InputSystem.GetButtonDown(MechCommandButton.Jump);
        var isAllowed = status.IsOnGround && status.State != MechMovementState.Boosting;
        return isRequested && isAllowed;
    }
}



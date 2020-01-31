using UnityEngine;

public abstract class MechMovementAction {
    private PlayerInputData input;
    private MechMovementStatus status;
    private MechMovementConfigData config;

    protected PlayerInputData Input { get => this.input; private set => this.input = value; }
    protected MechMovementStatus Status { get => this.status; private set => this.status = value; }
    protected MechMovementConfigData Config { get => this.config; private set => this.config = value; }

    public void Initialize(PlayerInputData input, MechMovementStatus status, MechMovementConfigData config) {
        this.Input = input;
        this.Status = status;
        this.Config = config;
    }

    public abstract MechRequestedMovement CalculateMovement();
    public abstract bool IsExecutable();
}

public class CrouchAction : MechMovementAction {
    public override MechRequestedMovement CalculateMovement() {
        var movement = new MechRequestedMovement {
            State = MechMovementState.Crouching,
            Motion = Vector3.zero,
            LegYaw = this.Status.LegYaw
        };
        return movement;
    }

    public override bool IsExecutable() {
        var isRequested = this.Input.Crouch;
        var state = this.Status.State;
        var isAllowed =
            state == MechMovementState.Acceling || state == MechMovementState.Braking ||
            state == MechMovementState.Crouching || state == MechMovementState.Idle ||
            state == MechMovementState.Walking;
        return isRequested && isAllowed;
    }
}

public class BoostAction {
    public MechRequestedMovement CalculateMovement(MechMovementStatus status, MechMovementConfigData config, BoosterConfigData boostConfig, ref BoosterEngineStatus engineStatus) {
        engineStatus.Gauge += 2f;
        var movement = new MechRequestedMovement();
        engineStatus.Gauge -= 28;
        engineStatus.ElapsedTime = 0f;
        var inertiaDirection = status.Velocity.normalized;
        //var inputDirection = new Vector3(InputSystem.GetMoveHorizontal(), 0f, InputSystem.GetMoveVertical());
        var inputDirection = Vector3.zero;
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

    public bool IsActivatable(MechMovementStatus status, MechMovementConfigData config, BoosterConfigData boostConfig, BoosterEngineStatus engineStatus) {
        var state = status.State;
        //var isRequested = InputSystem.GetButtonDown(MechCommandButton.Boost);
        var isRequested = false;
        var hasEnoughGauge = engineStatus.Gauge >= boostConfig.Consumption;
        var isAllowed = state != MechMovementState.Airborne;
        return isRequested && hasEnoughGauge && isAllowed;
    }
}

public class JumpAction : MechMovementAction {
    public override MechRequestedMovement CalculateMovement() {
        var command = new MechRequestedMovement();
        command.State = MechMovementState.Airborne;
        command.IsLeavingGround = true;
        //this.engine?.ShowJetFlame(0.4f, Vector3.forward);
        // Jumping power is proportial to current moving speed.
        command.Motion = new Vector3 {
            x = this.Status.Velocity.x,
            y = this.Config.BaseJumpSpeed * (1f + 0.002f * this.Status.Velocity.magnitude),
            z = this.Status.Velocity.z
        } * Time.deltaTime;
        return command;
    }

    public override bool IsExecutable() {
        var isRequested = this.Input.Jump;
        var isAllowed = this.Status.IsOnGround && this.Status.State != MechMovementState.Boosting;
        return isRequested && isAllowed;
    }
}

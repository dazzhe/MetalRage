using Unity.Mathematics;
using UnityEngine;

public abstract class MechMovementAction {
    private MechCommand input;
    private MechMovementStatus status;
    private MechMovementConfigData config;

    protected MechCommand Input { get => this.input; private set => this.input = value; }
    protected MechMovementStatus Status { get => this.status; private set => this.status = value; }
    protected MechMovementConfigData Config { get => this.config; private set => this.config = value; }

    public void Initialize(MechCommand input, MechMovementStatus status, MechMovementConfigData config) {
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
            LegYaw = this.Status.LegYaw,
            UseRawMotion = false
    };
        return movement;
    }

    public override bool IsExecutable() {
        var isRequested = this.Input.Crouch;
        var state = this.Status.State;
        var isAllowed =
            state == MechMovementState.BoostBraking ||
            state == MechMovementState.Crouching || state == MechMovementState.Stand ||
            state == MechMovementState.Walking;
        return isRequested && isAllowed;
    }
}

public class BoostAction {
    MechCommand input;
    public void Initialize(MechCommand input, ParticleSystem boosterEffect) {
        this.input = input;
    }

    public MechRequestedMovement CalculateMovement(MechMovementStatus status, MechMovementConfigData config, BoosterConfigData boostConfig, ref BoosterEngineStatus engineStatus) {
        var movement = new MechRequestedMovement();
        engineStatus.Gauge -= boostConfig.Consumption;
        engineStatus.ElapsedTime = 0f;
        var mechRotation = quaternion.Euler(0f, status.Yaw, 0f);
        var inertiaLocalDirection = math.mul(math.inverse(mechRotation), status.Velocity.normalized);
        var inputLocalDirection = math.float3(this.input.Move.x, 0f, this.input.Move.y);
        float3 boostLocalDirection;
        if (inputLocalDirection.z < 0 || math.lengthsq(inputLocalDirection) == 0) {
            // Mechs cannot boost backward.
            boostLocalDirection = math.float3(0f, 0f, 1f);
        } else if (inputLocalDirection.z > 0) {
            boostLocalDirection = inputLocalDirection;
        } else if (math.dot(inputLocalDirection, inertiaLocalDirection) < 0f) {
            boostLocalDirection = inputLocalDirection;
        } else {
            boostLocalDirection = inertiaLocalDirection;
        }
        movement.LegYaw = 0f;
        var boostDirection = math.mul(mechRotation, boostLocalDirection);
        movement.Motion = math.normalizesafe(boostDirection) * boostConfig.MaxSpeed * Time.deltaTime;
        movement.State = MechMovementState.BoostAcceling;
        return movement;
    }

    public bool IsExecutable(MechMovementStatus status, MechMovementConfigData config, BoosterConfigData boostConfig, BoosterEngineStatus engineStatus) {
        var state = status.State;
        var isRequested = this.input.BoostOneShot;
        var hasEnoughGauge = engineStatus.Gauge >= boostConfig.Consumption;
        var isAllowed = state != MechMovementState.Airborne;
        return isRequested && hasEnoughGauge && isAllowed;
    }
}

public class JumpAction : MechMovementAction {
    public override MechRequestedMovement CalculateMovement() {
        var command = new MechRequestedMovement {
            State = MechMovementState.Airborne,
            UseRawMotion = true,
            //this.engine?.ShowJetFlame(0.4f, Vector3.forward);
            // Jumping power is proportial to current moving speed.
            Motion = new Vector3 {
                x = this.Status.Velocity.x,
                y = this.Config.BaseJumpSpeed * (1f + 0.002f * this.Status.Velocity.magnitude),
                z = this.Status.Velocity.z
            } * Time.deltaTime
        };
        return command;
    }

    public override bool IsExecutable() {
        var isRequested = this.Input.Jump;
        var isAllowed = this.Status.IsOnGround && this.Status.State != MechMovementState.BoostAcceling;
        return isRequested && isAllowed;
    }
}

using UnityEngine;

public abstract class MechStateBehaviour {
    public abstract MechRequestedMovement ComputeMovement(MechCommand input, MechMovementStatus status, MechMovementConfigData config);
}

public class WalkStateBehaviour : MechStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechCommand command, MechMovementStatus status, MechMovementConfigData config) {
        var motion = new Vector3(status.Velocity.x, 0f, status.Velocity.z);
        motion.x += command.Move.x * config.WalkingAcceleration * Time.deltaTime;
        motion.z += command.Move.y * config.WalkingAcceleration * Time.deltaTime;
        if (command.Move.x == 0) {
            var xAbs = Mathf.Max(Mathf.Abs(motion.x) - config.WalkingDeceleration * Time.deltaTime, 0f);
            motion.x = motion.x > 0 ? xAbs : -xAbs;
        }
        if (command.Move.y == 0) {
            var zAbs = Mathf.Max(Mathf.Abs(motion.z) - config.WalkingDeceleration * Time.deltaTime, 0f);
            motion.z = motion.z > 0 ? zAbs : -zAbs;
        }
        motion = motion.magnitude > config.MaxWalkSpeed ? motion.normalized * config.MaxWalkSpeed : motion;
        var movement = new MechRequestedMovement {
            Motion = motion * Time.deltaTime,
            State = motion.magnitude == 0 ? MechMovementState.Idle : MechMovementState.Walking
        };
        var x = command.Move.x;
        var z = command.Move.y;
        movement.LegYaw = x > 0 && z > 0 ? 45f :
                      x > 0 && z == 0 ? 90f :
           (x == 0 && z > 0) || z < 0 ? 0f :
                       x < 0 && z > 0 ? -45f :
                      x < 0 && z == 0 ? -90f : status.LegYaw;
        return movement;
    }
}

public class BoostStateBehaviour {
    public MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config, BoosterConfigData boostConfig, ref BoosterEngineStatus engineStatus) {
        engineStatus.ElapsedTime += Time.deltaTime;
        var movement = new MechRequestedMovement();
        switch (status.State) {
            case MechMovementState.Acceling:
                var speed = boostConfig.Accel * engineStatus.ElapsedTime;
                speed = Mathf.Min(speed, boostConfig.MaxSpeed);
                movement.Motion = status.Velocity.normalized * speed * Time.deltaTime;
                movement.State = speed == boostConfig.MaxSpeed
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
                speed = status.Velocity.magnitude - 0.5f * config.BrakingDeceleration * Time.deltaTime;
                speed = Mathf.Max(speed, 0f);
                movement.Motion = status.Velocity.normalized * speed * Time.deltaTime;
                movement.State = speed == 0
                    ? MechMovementState.Walking
                    : MechMovementState.Braking;
                break;
        }
        return movement;
    }
}

public class AirborneStateBehaviour : MechStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechCommand input, MechMovementStatus status, MechMovementConfigData config) {
        var movement = new MechRequestedMovement {
            State = MechMovementState.Airborne
        };
        var accel = new Vector3(input.Move.x, 0f, input.Move.y).normalized * 30f;
        var velocity = status.Velocity + 0.5f * accel * Time.deltaTime;
        if (velocity.magnitude > config.MaxSpeedInAir) {
            velocity = velocity.normalized * config.MaxSpeedInAir;
        }
        movement.Motion = velocity * Time.deltaTime;
        movement.LegYaw = status.LegYaw;
        movement.IsLeavingGround = false;
        return movement;
    }
}

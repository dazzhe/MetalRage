using UnityEngine;

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

public class BoostStateBehaviour {
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

public class AirborneStateBehaviour : MechStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config) {
        var movement = new MechRequestedMovement {
            State = MechMovementState.Airborne
        };
        //float horizontalSpeed = new Vector2(status.Velocity.x, status.Velocity.z).magnitude;
        var accel = new Vector3(InputSystem.GetMoveHorizontal(), 0f, InputSystem.GetMoveVertical()).normalized * 40f;
        accel.y = -40f;
        movement.Motion = status.Velocity * Time.deltaTime + 0.5f * accel * Time.deltaTime * Time.deltaTime;
        movement.LegYaw = status.LegYaw;
        movement.IsLeavingGround = false;
        return movement;
    }
}

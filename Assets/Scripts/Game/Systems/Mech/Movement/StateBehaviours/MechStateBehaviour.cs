using UnityEngine;
using static Unity.Mathematics.math;

public abstract class MechStateBehaviour {
    public abstract MechRequestedMovement ComputeMovement(MechCommand input, MechMovementStatus status, MechMovementConfigData config);
}

public class WalkStateBehaviour : MechStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechCommand command, MechMovementStatus status, MechMovementConfigData config) {
        var localMotion = mul(Unity.Mathematics.quaternion.Euler(0f, -status.Yaw, 0f), float3(status.Velocity.x, 0f, status.Velocity.z));
        if (localMotion.x * command.Move.x < 0f) {
            localMotion.x = 0f;
        }
        if (localMotion.z * command.Move.y < 0f) {
            localMotion.z = 0f;
        }
        localMotion.x += command.Move.x * config.WalkingAcceleration * Time.deltaTime;
        localMotion.z += command.Move.y * config.WalkingAcceleration * Time.deltaTime;
        if (command.Move.x == 0) {
            var xAbs = Mathf.Max(Mathf.Abs(localMotion.x) - config.WalkingDeceleration * Time.deltaTime, 0f);
            localMotion.x = localMotion.x > 0 ? xAbs : -xAbs;
        }
        if (command.Move.y == 0) {
            var zAbs = Mathf.Max(Mathf.Abs(localMotion.z) - config.WalkingDeceleration * Time.deltaTime, 0f);
            localMotion.z = localMotion.z > 0 ? zAbs : -zAbs;
        }
        localMotion = Vector3.ClampMagnitude(localMotion, config.MaxWalkSpeed);
        var movement = new MechRequestedMovement {
            Motion = mul(Unity.Mathematics.quaternion.Euler(0f, status.Yaw, 0f), localMotion * Time.deltaTime),
            State = command.Move.magnitude == 0 ? MechMovementState.Stand : MechMovementState.Walking,
            UseRawMotion = false
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
    public static float brakingTime = 0f;
    public MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config, BoosterConfigData boostConfig, ref BoosterEngineStatus engineStatus) {
        engineStatus.ElapsedTime += Time.deltaTime;
        var movement = new MechRequestedMovement {
            UseRawMotion = false
        };
        switch (status.State) {
            case MechMovementState.BoostAcceling:
                movement.Motion = status.Velocity * Time.deltaTime;
                movement.State = engineStatus.ElapsedTime > boostConfig.Duration && status.IsOnGround
                    ? MechMovementState.BoostBraking
                    : MechMovementState.BoostAcceling;
                brakingTime = 0f;
                break;
            case MechMovementState.BoostBraking:
                var speed = status.Velocity.magnitude - boostConfig.BrakingDeceleration * Time.deltaTime;
                speed = Mathf.Max(speed, 0f);
                movement.Motion = status.Velocity.normalized * speed * Time.deltaTime;
                movement.State = speed == 0
                    ? MechMovementState.Stand
                    : MechMovementState.BoostBraking;
                brakingTime += Time.deltaTime;
                Debug.Log(brakingTime);
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
        if (velocity.magnitude > config.MaxFallSpeed) {
            velocity = velocity.normalized * config.MaxFallSpeed;
        }
        movement.Motion = velocity * Time.deltaTime;
        movement.LegYaw = status.LegYaw;
        movement.UseRawMotion = false;
        return movement;
    }
}

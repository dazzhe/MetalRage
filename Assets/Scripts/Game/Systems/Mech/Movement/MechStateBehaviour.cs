using Unity.Mathematics;
using UnityEngine;

public abstract class MechStateBehaviour {
    public abstract MechRequestedMovement ComputeMovement(MechCommand input, MechMovementStatus status, MechMovementConfigData config);
}

public class WalkStateBehaviour : MechStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechCommand command, MechMovementStatus status, MechMovementConfigData config) {
        var localVelocity = math.mul(quaternion.Euler(0f, -status.Yaw, 0f), math.float3(status.Velocity.x, 0f, status.Velocity.z));
        if (localVelocity.x * command.Move.x < 0f) {
            localVelocity.x = 0f;
        }
        if (localVelocity.z * command.Move.y < 0f) {
            localVelocity.z = 0f;
        }
        localVelocity.x += command.Move.x * config.WalkingAcceleration * Time.deltaTime;
        localVelocity.z += command.Move.y * config.WalkingAcceleration * Time.deltaTime;
        if (command.Move.x == 0) {
            var xAbs = Mathf.Max(Mathf.Abs(localVelocity.x) - config.WalkingDeceleration * Time.deltaTime, 0f);
            localVelocity.x = localVelocity.x > 0 ? xAbs : -xAbs;
        }
        if (command.Move.y == 0) {
            var zAbs = Mathf.Max(Mathf.Abs(localVelocity.z) - config.WalkingDeceleration * Time.deltaTime, 0f);
            localVelocity.z = localVelocity.z > 0 ? zAbs : -zAbs;
        }
        localVelocity = Vector3.ClampMagnitude(localVelocity, config.MaxWalkSpeed);
        var movement = new MechRequestedMovement {
            Velocity = math.mul(quaternion.Euler(0f, status.Yaw, 0f), localVelocity),
            State = command.Move.magnitude == 0 ? MechMovementState.Stand : MechMovementState.Walking,
            UseRawVelocity = false
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
            UseRawVelocity = false
        };
        switch (status.State) {
            case MechMovementState.BoostAcceling:
                movement.Velocity = status.Velocity;
                movement.State = engineStatus.ElapsedTime > boostConfig.Duration && status.IsOnGround
                    ? MechMovementState.BoostBraking
                    : MechMovementState.BoostAcceling;
                brakingTime = 0f;
                break;
            case MechMovementState.BoostBraking:
                var speed = status.Velocity.magnitude - boostConfig.BrakingDeceleration * Time.deltaTime;
                speed = Mathf.Max(speed, 0f);
                movement.Velocity = status.Velocity.normalized * speed;
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
        movement.Velocity = velocity;
        movement.LegYaw = status.LegYaw;
        movement.UseRawVelocity = false;
        return movement;
    }
}

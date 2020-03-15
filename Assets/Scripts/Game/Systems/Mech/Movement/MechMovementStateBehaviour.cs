using Unity.Mathematics;
using UnityEngine;

public abstract class MechMovementStateBehaviour {
    public abstract MechRequestedMovement ComputeMovement(MechCommand input, MechMovementStatus status, MechMovementConfigData config);
}

public class WalkStateBehaviour : MechMovementStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechCommand command, MechMovementStatus status, MechMovementConfigData config) {
        var input = new float3(command.Move.x, 0f, command.Move.y);
        var localVelocity = math.mul(quaternion.Euler(0f, -status.Yaw, 0f), math.float3(status.Velocity.x, 0f, status.Velocity.z));
        var isTurning = input * localVelocity < 0f;
        localVelocity.x = isTurning.x ? 0f : localVelocity.x;
        localVelocity.z = isTurning.z ? 0f : localVelocity.z;
        if (input.x == 0) {
            var xAbs = Mathf.Max(Mathf.Abs(localVelocity.x) - config.WalkingDeceleration * Time.deltaTime, 0f);
            localVelocity.x = localVelocity.x > 0 ? xAbs : -xAbs;
        } else {
            localVelocity.x += input.x * config.WalkingAcceleration * Time.deltaTime;
        }
        if (input.z == 0) {
            var zAbs = Mathf.Max(Mathf.Abs(localVelocity.z) - config.WalkingDeceleration * Time.deltaTime, 0f);
            localVelocity.z = localVelocity.z > 0 ? zAbs : -zAbs;
        } else {
            localVelocity.z += input.z * config.WalkingAcceleration * Time.deltaTime;
        }
        localVelocity = math.length(localVelocity) > config.MaxWalkSpeed
            ? math.normalizesafe(localVelocity) * config.MaxWalkSpeed
            : localVelocity;
        var movement = new MechRequestedMovement {
            Velocity = math.mul(quaternion.Euler(0f, status.Yaw, 0f), localVelocity),
            State = math.lengthsq(input) == 0 ? MechMovementState.Stand : MechMovementState.Walking,
            UseRawVelocity = false
        };
        return movement;
    }
}

public class BoostStateBehaviour {
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
                break;
            case MechMovementState.BoostBraking:
                var speed = status.Velocity.magnitude - boostConfig.BrakingDeceleration * Time.deltaTime;
                speed = Mathf.Max(speed, 0f);
                movement.Velocity = status.Velocity.normalized * speed;
                movement.State = speed == 0
                    ? MechMovementState.Stand
                    : MechMovementState.BoostBraking;
                break;
        }
        return movement;
    }
}

public class AirborneStateBehaviour : MechMovementStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechCommand input, MechMovementStatus status, MechMovementConfigData config) {
        var movement = new MechRequestedMovement {
            State = MechMovementState.Airborne
        };
        var accel = new Vector3(input.Move.x, 0f, input.Move.y).normalized * 30f;
        var velocity = status.Velocity + accel * Time.deltaTime;
        if (velocity.magnitude > config.MaxFallSpeed) {
            velocity = velocity.normalized * config.MaxFallSpeed;
        }
        movement.Velocity = velocity;
        movement.LegYaw = status.LegYaw;
        movement.UseRawVelocity = false;
        return movement;
    }
}

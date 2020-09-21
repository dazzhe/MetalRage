using Unity.Mathematics;
using UnityEngine;

public abstract class MechMovementStateBehaviour {
    public abstract MechRequestedMovement ComputeMovement(MechCommand input, MechMovementStatus status, MechMovementConfigData config, float dt);
}

public class WalkStateBehaviour : MechMovementStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechCommand command, MechMovementStatus status, MechMovementConfigData config, float dt) {
        var input = new float3(command.Move.x, 0f, command.Move.y);
        var localVelocity = math.mul(quaternion.Euler(0f, -status.Yaw, 0f), math.float3(status.Velocity.x, 0f, status.Velocity.z));
        var isTurning = input * localVelocity < 0f;
        localVelocity.x = isTurning.x ? 0f : localVelocity.x;
        localVelocity.z = isTurning.z ? 0f : localVelocity.z;
        if (input.x == 0) {
            var xAbs = Mathf.Max(Mathf.Abs(localVelocity.x) - config.WalkingDeceleration * dt, 0f);
            localVelocity.x = localVelocity.x > 0 ? xAbs : -xAbs;
        } else {
            localVelocity.x += input.x * config.WalkingAcceleration * dt;
        }
        if (input.z == 0) {
            var zAbs = Mathf.Max(Mathf.Abs(localVelocity.z) - config.WalkingDeceleration * dt, 0f);
            localVelocity.z = localVelocity.z > 0 ? zAbs : -zAbs;
        } else {
            localVelocity.z += input.z * config.WalkingAcceleration * dt;
        }
        localVelocity = math.length(localVelocity) > config.MaxWalkSpeed
            ? math.normalizesafe(localVelocity) * config.MaxWalkSpeed
            : localVelocity;
        var movement = new MechRequestedMovement {
            Velocity = math.mul(quaternion.Euler(0f, status.Yaw, 0f), localVelocity),
            State = math.lengthsq(input) == 0 ? MechMovementState.Stand : MechMovementState.Walking,
            ShouldJump = false
        };
        return movement;
    }
}

public class BoostStateBehaviour {
    public MechRequestedMovement ComputeMovement(MechMovementStatus status, MechMovementConfigData config, float dt, BoosterConfigData boostConfig, ref BoosterEngineStatus engineStatus) {
        engineStatus.ElapsedTime += dt;
        var movement = new MechRequestedMovement {
        };
        var speed = math.length(status.Velocity);
        movement.Velocity = status.Velocity;
        movement.Velocity.y = 0f;
        switch (status.State) {
            case MechMovementState.BoostAcceling:
                movement.Velocity = math.normalizesafe(movement.Velocity) * speed;
                movement.State = engineStatus.ElapsedTime > boostConfig.Duration && status.IsOnGround
                ? MechMovementState.BoostBraking
                : MechMovementState.BoostAcceling;
                break;
            case MechMovementState.BoostBraking:
                speed -= boostConfig.BrakingDeceleration * dt;
                speed = Mathf.Max(speed, 0f);
                movement.Velocity = math.normalizesafe(movement.Velocity) * speed;
                movement.State = speed == 0
                    ? MechMovementState.Stand
                    : MechMovementState.BoostBraking;
                break;
        }
        return movement;
    }
}

public class AirborneStateBehaviour : MechMovementStateBehaviour {
    public override MechRequestedMovement ComputeMovement(MechCommand input, MechMovementStatus status, MechMovementConfigData config, float dt) {
        var movement = new MechRequestedMovement {
            State = MechMovementState.Airborne,
        };
        var accel = math.normalizesafe(new float3(input.Move.x, 0f, input.Move.y)) * 30f;
        var localVelocity = math.mul(quaternion.Euler(0f, -status.Yaw, 0f), status.Velocity);
        localVelocity.y = 0f;
        localVelocity += accel * dt;
        if (math.length(localVelocity) > config.MaxMovementSpeedInAir) {
            localVelocity = math.normalizesafe(localVelocity) * config.MaxMovementSpeedInAir;
        }
        movement.Velocity = math.mul(quaternion.Euler(0f, status.Yaw, 0f), localVelocity);
        movement.LegYaw = status.LegYaw;
        return movement;
    }
}

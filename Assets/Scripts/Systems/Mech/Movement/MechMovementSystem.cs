using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
public struct MechRequestedMovement {
    public Vector3 Motion;
    public MechMovementState State;
    public float LegYaw;
    public Bool IsLeavingGround;
}

// TODO : Boost while crouching
// TODO : Multiple input at the same frame
[UpdateAfter(typeof(MechCommandSystem))]
public class MechMovementSystem : ComponentSystem {
    private static BoostAction boostAction = new BoostAction();
    private static BoostStateBehaviour boostStateBehaviour = new BoostStateBehaviour();
    private static MechMovementAction[] actions = new MechMovementAction[] {
        new CrouchAction(),
        new JumpAction(),
    };
    private static Dictionary<MechMovementState, MechStateBehaviour> stateBehaviours = new Dictionary<MechMovementState, MechStateBehaviour> {
        { MechMovementState.Idle, new WalkStateBehaviour() },
        { MechMovementState.Walking, new WalkStateBehaviour() },
        { MechMovementState.Crouching, new WalkStateBehaviour() },
        { MechMovementState.Airborne, new AirborneStateBehaviour() },
    };

    protected override void OnUpdate() {
        this.Entities.ForEach((Entity entity, CharacterController characterController, ref MechCommand command, ref MechMovementStatus status, ref MechMovementConfigData config, ref BoosterConfigData boostConfig, ref BoosterEngineStatus engineStatus) => {
            status.Yaw = characterController.transform.eulerAngles.y + command.DeltaLook.x * Preferences.Sensitivity.GetFloat();
            characterController.transform.eulerAngles = new Vector3(0f, status.Yaw % 360f, 0f);
            status.Pitch -= command.DeltaLook.y * Preferences.Sensitivity.GetFloat();
            status.Pitch = Mathf.Clamp(status.Pitch, config.MinPitch, config.MaxPitch);

            MechRequestedMovement requestedMovement = new MechRequestedMovement();
            foreach (var action in actions) {
                action.Initialize(command, status, config);
            }
            boostAction.Initialize(command, this.EntityManager.GetComponentObject<MechComponent>(entity).BoosterEffect);
            var activatedActionExists = false;
            if (boostAction.IsActivatable(status, config, boostConfig, engineStatus)) {
                requestedMovement = boostAction.CalculateMovement(status, config, boostConfig, ref engineStatus);
                activatedActionExists = true;
            } else {
                foreach (var action in actions) {
                    if (action.IsExecutable()) {
                        requestedMovement = action.CalculateMovement();
                        activatedActionExists = true;
                        break;
                    }
                }
            }
            if (!activatedActionExists) {
                switch (status.State) {
                    case MechMovementState.Boosting:
                    case MechMovementState.Braking:
                        requestedMovement = boostStateBehaviour.ComputeMovement(status, config, boostConfig, ref engineStatus);
                        break;
                    default:
                        requestedMovement = stateBehaviours[status.State].ComputeMovement(command, status, config);
                        break;
                }
            }
            var prevPosition = characterController.transform.position;
            var motion = characterController.transform.TransformVector(requestedMovement.Motion);
            var canMoveOnGround = false;
            if (status.IsOnGround && !requestedMovement.IsLeavingGround) {
                // TODO: Maximum slope angle
                canMoveOnGround = true;
                var testMotion = motion;
                var horizontalMotionMag = new Vector3(requestedMovement.Motion.x, 0, requestedMovement.Motion.z).magnitude;
                testMotion.y =
                    -horizontalMotionMag * this.Time.DeltaTime +
                    0.5f * config.Gravity * this.Time.DeltaTime * this.Time.DeltaTime;
                var currentPosition = characterController.transform.position;
                characterController.Move(testMotion);
                if (!characterController.isGrounded) {
                    characterController.transform.position = currentPosition;
                    canMoveOnGround = false;
                }
            }
            if (!canMoveOnGround) {
                if (!requestedMovement.IsLeavingGround) {
                    motion.y =
                        status.Velocity.y * this.Time.DeltaTime +
                        0.5f * config.Gravity * this.Time.DeltaTime * this.Time.DeltaTime;
                    motion.y = Mathf.Clamp(motion.y, -config.MaxSpeedInAir, config.MaxSpeedInAir);
                    characterController.Move(motion);
                } else {
                    characterController.Move(motion);
                }
            }
            status.Velocity = Quaternion.Inverse(characterController.transform.rotation) * (characterController.transform.position - prevPosition) / this.Time.DeltaTime;
            status.LegYaw = Mathf.Lerp(status.LegYaw, requestedMovement.LegYaw, 10f * this.Time.DeltaTime);
            status.IsOnGround = requestedMovement.IsLeavingGround ? false : characterController.isGrounded;
            if (status.IsOnGround && requestedMovement.State == MechMovementState.Airborne) {
                status.State = MechMovementState.Idle;
            } else if (!status.IsOnGround && requestedMovement.State != MechMovementState.Boosting) {
                status.State = MechMovementState.Airborne;
            } else {
                status.State = requestedMovement.State;
            }
        });
    }
}

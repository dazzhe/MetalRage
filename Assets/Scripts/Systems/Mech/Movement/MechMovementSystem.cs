using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum MechMovementState {
    Idle,
    Walking,
    Crouching,
    Airborne,
    Acceling,
    Boosting,
    Braking
}

[System.Serializable]
public struct MechMovementConfigData : IComponentData {
    public float MinPitch;
    public float MaxPitch;
    public float MaxSpeedInAir;
    public float BrakingDeceleration;
    [Header("Walk")]
    public float WalkingAcceleration;
    public float WalkingDeceleration;
    public float MaxWalkSpeed;
    [Header("Jump")]
    public float BaseJumpSpeed;
}

// TODO : Boost while crouching
// TODO : Multiple input at the same frame
[UpdateAfter(typeof(MechInputSystem))]
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

    private EntityQuery query;

    protected override void OnCreate() {
        this.query = GetEntityQuery(typeof(PlayerInputData));
    }

    protected override void OnUpdate() {
        var input = this.query.GetSingleton<PlayerInputData>();
        this.Entities.ForEach((Entity entity, CharacterController characterController, ref MechMovementStatus status, ref MechMovementConfigData config, ref BoosterConfigData boostConfig, ref BoosterEngineStatus engineStatus) => {
            status.Yaw = characterController.transform.eulerAngles.y + input.DeltaLook.x * Preferences.Sensitivity.GetFloat();
            characterController.transform.eulerAngles = new Vector3(0f, status.Yaw % 360f, 0f);
            status.Pitch -= input.DeltaLook.y * Preferences.Sensitivity.GetFloat();
            status.Pitch = Mathf.Clamp(status.Pitch, config.MinPitch, config.MaxPitch);

            MechMovementAction activatableAction = null;
            MechRequestedMovement requestedMovement;
            foreach (var action in actions) {
                action.Initialize(input, status, config);
            }
            boostAction.Initialize(input, this.EntityManager.GetComponentObject<MechComponent>(entity).BoosterEffect);
            if (boostAction.IsActivatable(status, config, boostConfig, engineStatus)) {
                requestedMovement = boostAction.CalculateMovement(status, config, boostConfig, ref engineStatus);
            } else {
                foreach (var action in actions) {
                    if (action.IsExecutable()) {
                        activatableAction = action;
                        break;
                    }
                }
                if (activatableAction != null) {
                    requestedMovement = activatableAction.CalculateMovement();
                } else {
                    switch (status.State) {
                        case MechMovementState.Acceling:
                        case MechMovementState.Boosting:
                        case MechMovementState.Braking:
                            requestedMovement = boostStateBehaviour.ComputeMovement(status, config, boostConfig, ref engineStatus);
                            break;
                        default:
                            requestedMovement = stateBehaviours[status.State].ComputeMovement(input, status, config);
                            break;
                    }
                }
            }
            var prevPosition = characterController.transform.position;
            characterController.Move(characterController.transform.TransformVector(requestedMovement.Motion));
            status.Velocity = Quaternion.Inverse(characterController.transform.rotation) * (characterController.transform.position - prevPosition) / this.Time.DeltaTime;
            status.State = requestedMovement.State;
            status.LegYaw = Mathf.Lerp(status.LegYaw, requestedMovement.LegYaw, 10f * this.Time.DeltaTime);
            status.IsOnGround = requestedMovement.IsLeavingGround ? false : characterController.isGrounded;
            if (status.IsOnGround && status.State == MechMovementState.Airborne) {
                status.State = MechMovementState.Idle;
            }
        });
    }
}

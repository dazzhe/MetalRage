using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct MechMovementConfigData : IComponentData {
    [Header("Walk")]
    public float WalkAccel;
    public float Decel;
    public float MaxWalkSpeed;
    [Header("Jump")]
    public float BaseJumpSpeed;
}

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
        this.Entities.ForEach((CharacterController characterController, ref MechMovementStatus status, ref MechMovementConfigData config, ref BoostActionConfigData boostConfig, ref BoosterEngineStatus engineStatus) => {
            MechMovementAction activatableAction = null;
            MechRequestedMovement requestedMovement;
            foreach (var action in actions) {
                action.Initialize(input, status, config);
            }
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
                            requestedMovement = stateBehaviours[status.State].ComputeMovement(status, config);
                            break;
                    }
                }
            }
            var prevPosition = characterController.transform.position;
            characterController.Move(requestedMovement.Motion);
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

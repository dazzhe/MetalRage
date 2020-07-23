using System.Collections.Generic;
using Unity.Entities;

// TODO : Boost while crouching
// TODO : Multiple input at the same frame
[UpdateAfter(typeof(MechCommandSystem))]
public class MechMovementRequestSystem : ComponentSystem {
    private static BoostAction boostAction = new BoostAction();
    private static BoostStateBehaviour boostStateBehaviour = new BoostStateBehaviour();
    private static MechMovementAction[] actions = new MechMovementAction[] {
        new CrouchAction(),
        new JumpAction(),
    };
    private static Dictionary<MechMovementState, MechMovementStateBehaviour> stateBehaviours = new Dictionary<MechMovementState, MechMovementStateBehaviour> {
        { MechMovementState.Stand, new WalkStateBehaviour() },
        { MechMovementState.Walking, new WalkStateBehaviour() },
        { MechMovementState.Crouching, new WalkStateBehaviour() },
        { MechMovementState.Airborne, new AirborneStateBehaviour() },
    };

    protected override void OnUpdate() {
        this.Entities.ForEach((
            ref MechCommand command,
            ref MechMovementStatus status,
            ref MechRequestedMovement requestedMovement,
            ref MechMovementConfigData config,
            ref BoosterConfigData boostConfig,
            ref BoosterEngineStatus engineStatus) => {
                foreach (var action in actions) {
                    action.Initialize(command, status, config);
                }
                boostAction.Initialize(command, null);
                //boostAction.Initialize(command, this.EntityManager.GetComponentObject<MechComponent>(entity).BoosterEffect);
                var hasExecutedAction = false;
                if (boostAction.IsExecutable(status, config, boostConfig, engineStatus)) {
                    requestedMovement = boostAction.CalculateMovement(status, config, boostConfig, ref engineStatus);
                    hasExecutedAction = true;
                } else {
                    foreach (var action in actions) {
                        if (action.IsExecutable()) {
                            requestedMovement = action.CalculateMovement();
                            hasExecutedAction = true;
                            break;
                        }
                    }
                }
                if (!hasExecutedAction) {
                    switch (status.State) {
                        case MechMovementState.BoostAcceling:
                        case MechMovementState.BoostBraking:
                            requestedMovement = boostStateBehaviour.ComputeMovement(status, config, boostConfig, ref engineStatus);
                            break;
                        default:
                            requestedMovement = stateBehaviours[status.State].ComputeMovement(command, status, config);
                            break;
                    }
                }
            });
    }
}

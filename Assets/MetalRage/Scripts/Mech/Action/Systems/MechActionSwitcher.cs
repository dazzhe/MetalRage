using System.Linq;
using Unity.Entities;

class MechActionSwitcher : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((DynamicBuffer<MechActionEntity> actionEntityBuffer) => {
            var actions = actionEntityBuffer.AsNativeArray().Select(entity =>
                this.EntityManager.GetComponentData<MechAction>(entity.Value)
            ).ToArray();
            DeactivateCompletedActions(ref actions);
            ActivateActions(actions);
            var entities = actionEntityBuffer.AsNativeArray().Select(entity => entity.Value).ToArray();
            Apply(entities, actions);
        });
    }

    private void DeactivateCompletedActions(ref MechAction[] actions) {
        for (int i = 0; i < actions.Length; ++i) {
            var action = actions[i];
            if (action.IsActive && action.State != ActionState.Active) {
                Deactivate(ref action);
            }
        }
    }

    private void ActivateActions(MechAction[] actions) {
        for (int i = 0; i < actions.Length; i++) {
            if (actions[i].IsActive) {
                continue;
            }
            if (actions[i].State != ActionState.RequestingActivation) {
                continue;
            }
            var isBlocked = actions.Any(anotherAction => {
                var isBlockingAction = anotherAction.BlockingTag.HasFlag(actions[i].Tag);
                return anotherAction.IsActive & isBlockingAction;
            });
            if (!isBlocked) {
                Activate(ref actions[i]);
                continue;
            }
            var canActivate = true;
            for (int j = 0; j < actions.Length; j++) {
                var anotherAction = actions[j];
                var isBlockedByAnotherAction = anotherAction.IsActive && anotherAction.BlockingTag.HasFlag(actions[i].Tag);
                if (isBlockedByAnotherAction) {
                    var canInterrupt = actions[i].InterruptibleTag.HasFlag(anotherAction.Tag);
                    if (canInterrupt) {
                        Deactivate(ref anotherAction);
                    } else {
                        RequestDeactivation(ref anotherAction);
                        canActivate = false;
                    }
                }
            }
            if (canActivate) {
                Activate(ref actions[i]);
            }
        }
    }

    private void Activate(ref MechAction action) {
        action.IsActive = true;
        action.IsDeactivationRequested = false;
    }

    private void Deactivate(ref MechAction action) {
        action.IsActive = false;
    }

    private void RequestDeactivation(ref MechAction action) {
        action.IsDeactivationRequested = true;
    }

    private void Apply(Entity[] entities, MechAction[] actions) {
        for (int i = 0; i < entities.Length; ++i) {
            this.EntityManager.SetComponentData(entities[i], actions[i]);
        }
    }
}

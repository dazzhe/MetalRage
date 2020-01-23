using System.Linq;
using Unity.Entities;

class MechActionActivator : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((DynamicBuffer<MechActionEntity> actionEntityBuffer) => {
            var actions = actionEntityBuffer.AsNativeArray().Select(entity =>
                this.EntityManager.GetComponentData<MechAction>(entity.Value)
            ).ToArray();
            var constraints = actionEntityBuffer.AsNativeArray().Select(entity =>
                this.EntityManager.GetComponentData<ActionConstraint>(entity.Value)
            ).ToArray();
            Deactivate(actions);
            Activate(actions, constraints);
            var entities = actionEntityBuffer.AsNativeArray().Select(entity => entity.Value).ToArray();
            for (int i = 0; i < entities.Length; ++i) {
                this.EntityManager.SetComponentData(entities[i], actions[i]);
            }
        });
    }

    private void Deactivate(MechAction[] actions) {
        for (int i = 0; i < actions.Length; ++i) {
            if (actions[i].IsActive && actions[i].State != ActionState.Running) {
                actions[i].IsActive = false;
            }
        }
    }

    private void DeactivateByForce(ref MechAction action) {
        action.IsActive = false;
    }

    private void Activate(MechAction[] actions, ActionConstraint[] constraints) {
        var isDeactivatedByForceArray = new bool[actions.Length];
        for (int i = 0; i < actions.Length; ++i) {
            var needActivation =
                !actions[i].IsActive && !isDeactivatedByForceArray[i] &&
                actions[i].State == ActionState.WaitingActivation;
            if (!needActivation) {
                continue;
            }
            if (!CheckIsBlocked(actions, i)) {
                actions[i].IsActive = true;
                actions[i].IsDeactivationRequested = false;
                for (int j = 0; j < actions.Length; ++j) {
                    if (i == j) {
                        continue;
                    }
                    if (actions[j].IsActive && constraints[j].ExecutionCancellingTag.HasFlag(constraints[j].Tag)) {
                        DeactivateByForce(ref actions[j]);
                        isDeactivatedByForceArray[j] = true;
                    }
                }
            }
        }
    }

    private bool CheckIsBlocked(MechAction[] actions, int index) {
        var isBlocked = false;
        for (int i = 0; i < actions.Length; ++i) {
            if (index == i) {
                continue;
            }
            var isBlockingAction = actions[i].ActivationBlockingTag.HasFlag(actions[i].Tag);
            if (actions[i].IsActive & isBlockingAction) {
                isBlocked = true;
                actions[i].IsDeactivationRequested = true;
            }
        }
        return isBlocked;
    }

    private void RequestDeactivation(ref MechAction action) {
        action.IsDeactivationRequested = true;
    }
}

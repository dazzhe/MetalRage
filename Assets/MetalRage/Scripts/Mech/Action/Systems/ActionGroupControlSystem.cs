using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

public class ActionGroupControlSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((DynamicBuffer<MechActionEntity> actionEntityBuffer) => {
            var actions = actionEntityBuffer.AsNativeArray().Select(entity =>
                this.EntityManager.GetComponentData<MechAction>(entity.Value)
            ).ToArray();
            var constraints = actionEntityBuffer.AsNativeArray().Select(entity =>
                this.EntityManager.GetComponentData<ActionExecutionConstraint>(entity.Value)
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
            if (!actions[i].IsReadyToExecute) {
                actions[i].IsExecutionAllowed = false;
            }
        }
    }

    private void DeactivateForcibly(ref MechAction action) {
        action.IsExecutionAllowed = false;
    }

    private void Activate(MechAction[] actions, ActionExecutionConstraint[] constraints) {
        var isForciblyDeactivatedArray = new bool[actions.Length];
        for (int i = 0; i < actions.Length; ++i) {
            var needActivation = !actions[i].IsExecutionAllowed && actions[i].IsReadyToExecute;
            var isForciblyDeactivated = isForciblyDeactivatedArray[i];
            if (!needActivation || isForciblyDeactivated) {
                continue;
            }
            var blockingActionIndices = GetBlockingActionIndices(actions, constraints, i);
            foreach (var blockingIndex in blockingActionIndices) {
                actions[blockingIndex].IsInterruptionRequested = true;
            }
            if (blockingActionIndices.Length > 0) {
                actions[i].IsExecutionAllowed = true;
                actions[i].IsInterruptionRequested = false;
                for (int j = 0; j < actions.Length; ++j) {
                    if (i == j) {
                        continue;
                    }
                    if (actions[j].IsExecutionAllowed && constraints[j].IsForciblyBlocking(constraints[i])) {
                        DeactivateForcibly(ref actions[j]);
                        isForciblyDeactivatedArray[j] = true;
                    }
                }
            }
        }
    }

    private int[] GetBlockingActionIndices(MechAction[] actions, ActionExecutionConstraint[] constraints, int index) {
        var blockingActionIndices = new List<int>();
        for (int i = 0; i < actions.Length; ++i) {
            if (index == i) {
                continue;
            }
            if (actions[i].IsExecutionAllowed & constraints[i].IsBlocking(constraints[index])) {
                blockingActionIndices.Add(i);
            }
        }
        return blockingActionIndices.ToArray();
    }
}

using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct ActionExecutionConstraint : IComponentData {
    public ActionTag Tag;
    [Tooltip("Other inactive(didn't execute in the last Update) actions with selected tags cannot be activated when this action is active.")]
    public ActionTag BlockingTag;
    [Tooltip("Other active(executed in the last Update) actions with selected tags are deactivated on the activation of this action and cannot be activated.")]
    public ActionTag ForciblyBlockingTag;

    public bool IsBlocking(ActionExecutionConstraint constraint) {
        return this.BlockingTag.HasFlag(constraint.Tag) || this.ForciblyBlockingTag.HasFlag(constraint.Tag);
    }

    public bool IsForciblyBlocking(ActionExecutionConstraint constraint) {
        return this.ForciblyBlockingTag.HasFlag(constraint.Tag);
    }
}

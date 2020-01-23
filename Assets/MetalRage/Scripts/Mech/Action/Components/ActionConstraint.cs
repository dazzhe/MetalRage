using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct ActionConstraint : IComponentData {
    [SerializeField]
    private ActionTag tag;
    [Tooltip("Other inactive actions with selected tags cannot be activated when this action is active.")]
    [SerializeField]
    private ActionTag blockingTag;
    [Tooltip("Other active actions with selected tags are deactivated on the activation of this action and cannot be activated.")]
    [SerializeField]
    private ActionTag forciblyBlockingTag;

    public ActionTag Tag { get => this.tag; set => this.tag = value; }
    public ActionTag BlockingTag { get => this.blockingTag; set => this.blockingTag = value; }
    public ActionTag ForciblyBlockingTag { get => this.forciblyBlockingTag; set => this.forciblyBlockingTag = value; }

    public bool IsBlocking(ActionConstraint constraint) {
        return this.blockingTag.HasFlag(constraint.Tag) || this.forciblyBlockingTag.HasFlag(constraint.Tag);
    }

    public bool IsForciblyBlocking(ActionConstraint constraint) {
        return this.forciblyBlockingTag.HasFlag(constraint.Tag);
    }
}

using Unity.Entities;
using UnityEngine;

public struct ActionConstraint : IComponentData {
    [SerializeField]
    private ActionTag tag;
    [SerializeField]
    private ActionTag activationBlockingTag;
    [SerializeField]
    private ActionTag executionCancellingTag;

    public ActionTag Tag { get => this.tag; set => this.tag = value; }
    public ActionTag ActivationBlockingTag { get => this.activationBlockingTag; set => this.activationBlockingTag = value; }
    public ActionTag ExecutionCancellingTag { get => this.executionCancellingTag; set => this.executionCancellingTag = value; }

    public bool IsBlocking(ActionConstraint constraint) {
        return this.activationBlockingTag.HasFlag(constraint.Tag);
    }

    public bool 
}

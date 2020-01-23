
using Unity.Entities;
using UnityEngine;

public abstract class MechActionConfigBase : ScriptableObject {
    [SerializeField]
    private ActionTag tag;
    [Tooltip("Other inactive actions with selected tags cannot activate when this action is active.")]
    [SerializeField]
    protected ActionTag activationBlockingTag;
    [Tooltip("Other active actions with selected tags are deactivated on the activation of this action.")]
    [SerializeField]
    protected ActionTag executionCancellingTag;

    protected ActionTag Tag { get => this.tag; set => this.tag = value; }
    public ActionTag ActivationBlockingTag { get => this.activationBlockingTag; set => this.activationBlockingTag = value; }
    public ActionTag ExecutionCancellingTag { get => this.executionCancellingTag; set => this.executionCancellingTag = value; }

    //public Entity CreateEntity(EntityManager entityManager, Entity owner) {
    //    var entity = entityManager.CreateEntity();
    //    var action = new MechAction {
    //        Owner = owner
    //    };
    //    var constraintConfig = new ActionConstraintConfig {
    //        Tag = this.tag,
    //        ActivationBlockingTag = this.ActivationBlockingTag,
    //        ExecutionCancellingTag = this.ExecutionCancellingTag,
    //    };
    //    entityManager.AddComponentData(entity, action);
    //    entityManager.AddComponentData(entity, action);
    //    AddExtraComponents(entityManager, entity);
    //    return entity;
    //}

    //protected abstract void AddExtraComponents(EntityManager entityManager, Entity entity);

    //public MechActionEntity CreateBufferElement(EntityManager entityManager, Entity owner) {
    //    return new MechActionEntity {
    //        Value = CreateEntity(entityManager, owner)
    //    };
    //}
}

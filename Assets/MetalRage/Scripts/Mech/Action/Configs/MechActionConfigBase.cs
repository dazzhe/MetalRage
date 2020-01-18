using Unity.Entities;
using UnityEngine;

// Avoid using generics and interface to show in the inspector with simple implementation.
public abstract class MechActionConfigBase : ScriptableObject {
    [SerializeField]
    private ActionTag tag;
    [SerializeField]
    protected ActionTag blockingTag;
    [SerializeField]
    protected ActionTag interruptibleTag;

    protected ActionTag Tag { get => this.tag; set => this.tag = value; }
    public ActionTag BlockingTag { get => this.blockingTag; set => this.blockingTag = value; }
    public ActionTag InterruptibleTag { get => this.interruptibleTag; set => this.interruptibleTag = value; }

    public Entity CreateEntity(EntityManager entityManager, Entity owner) {
        var entity = entityManager.CreateEntity();
        var action = new MechAction {
            Tag = this.tag,
            BlockingTag = this.BlockingTag,
            InterruptibleTag = this.InterruptibleTag,
            Owner = owner
        };
        entityManager.AddComponentData(entity, action);
        AddConfig(entityManager, ref entity);
        return entity;
    }

    protected abstract void AddConfig(EntityManager entityManager, ref Entity entity);

    public MechActionEntity CreateBufferElement(EntityManager entityManager, Entity owner) {
        return new MechActionEntity {
            Value = CreateEntity(entityManager, owner)
        };
    }
}

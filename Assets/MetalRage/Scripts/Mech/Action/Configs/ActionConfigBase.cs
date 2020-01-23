using Unity.Entities;
using UnityEngine;

public abstract class ActionConfigBase : ScriptableObject {
    [SerializeField]
    private ActionConstraint constraint;

    public ActionConstraint Constraint { get => this.constraint; set => this.constraint = value; }

    public abstract MechActionEntity CreateBufferElement(EntityManager entityManager, Entity owner);
}

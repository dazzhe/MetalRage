using Unity.Entities;
using UnityEngine;

public class ActionConfig<TConfigData> : ActionConfigBase
    where TConfigData : struct, IComponentData {
    [SerializeField]
    private TConfigData data = default;

    public override MechActionEntity CreateBufferElement(EntityManager entityManager, Entity owner) {
        var entity = entityManager.CreateEntity();
        var action = new MechAction {
            Owner = owner
        };
        entityManager.AddComponentData(entity, action);
        entityManager.AddComponentData(entity, this.Constraint);
        entityManager.AddComponentData(entity, this.data);
        return new MechActionEntity {
            Value = entity
        };
    }
}

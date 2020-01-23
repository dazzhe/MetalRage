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

[CreateAssetMenu(fileName = "WalkConfig", menuName = "MetalRage/Mech/Actions/WalkConfig")]
public class WalkActionConfig : ActionConfig<WalkActionConfigData> { }

[CreateAssetMenu(fileName = "CrouchConfig", menuName = "MetalRage/Mech/Actions/CrouchConfig")]
public class CrouchActionConfig : ActionConfig<CrouchActionConfigData> { }

[CreateAssetMenu(fileName = "BoostConfig", menuName = "MetalRage/Mech/Actions/BoostConfig")]
public class BoostActionConfig : ActionConfig<CrouchActionConfigData> { }

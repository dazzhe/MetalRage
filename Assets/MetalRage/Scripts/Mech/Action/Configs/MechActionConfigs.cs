using Unity.Entities;
using UnityEngine;

public class MechActionConfig<TConfigData> : MechActionConfigBase
    where TConfigData : struct, IComponentData {
    [SerializeField]
    private TConfigData data = default;

    protected override void AddConfigData(EntityManager entityManager, Entity entity) {
        entityManager.AddComponentData(entity, this.data);
    }
}

[CreateAssetMenu(fileName = "WalkConfig", menuName = "MetalRage/Mech/Actions/WalkConfig")]
public class WalkActionConfig : MechActionConfig<WalkActionConfigData> { }

[CreateAssetMenu(fileName = "CrouchConfig", menuName = "MetalRage/Mech/Actions/CrouchConfig")]
public class CrouchActionConfig : MechActionConfig<CrouchActionConfigData> { }

[CreateAssetMenu(fileName = "BoostConfig", menuName = "MetalRage/Mech/Actions/BoostConfig")]
public class BoostActionConfig : MechActionConfig<CrouchActionConfigData> { }

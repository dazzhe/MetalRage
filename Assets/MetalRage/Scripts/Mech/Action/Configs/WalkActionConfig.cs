using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "WalkConfig", menuName = "MetalRage/Mech/Actions/Walk")]
public class WalkActionConfig : MechActionConfigBase {
    [SerializeField]
    private WalkActionConfigData data;

    protected override void AddConfig(EntityManager entityManager, ref Entity entity) {
        entityManager.AddComponentData(entity, this.data);
    }
}

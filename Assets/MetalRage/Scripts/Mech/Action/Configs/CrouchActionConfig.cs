using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "CrouchConfig", menuName = "MetalRage/Mech/Action/Crouch")]
public class CrouchActionConfig : MechActionConfigBase {
    [SerializeField]
    private CrouchActionConfigData data;

    protected override void AddConfig(EntityManager entityManager, ref Entity entity) {
        entityManager.AddComponentData(entity, this.data);
    }
}

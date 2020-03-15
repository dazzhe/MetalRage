using UnityEngine;
using Unity.Entities;

public class MechAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    [SerializeField]
    private Transform cameraTarget;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        var mech = new Mech {
            MaxHP = 288,
            HP = 288,
            Armor = 1,
            CameraOffset = this.cameraTarget.localPosition
        };
        dstManager.AddComponentData(entity, mech);
    }
}

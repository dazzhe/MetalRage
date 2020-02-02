using Unity.Entities;
using UnityEngine;

public class PlayerCameraAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponentData(entity, new PlayerCamera());
        dstManager.AddComponentData(entity, new PlayerCameraCommand());
    }
}

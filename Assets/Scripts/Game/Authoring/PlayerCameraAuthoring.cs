using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerCameraAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponentData(entity, new PlayerCamera {
            forwardOffsetFactor = 4.7f,
            leanLength = 5f,
            BaseCameraOffset = new Vector3(0f, 0.5f, -5.5f)
        });
        dstManager.AddComponentData(entity, new PlayerCameraCommand());
        dstManager.AddComponentData(entity, new CopyTransformToGameObject());
    }
}

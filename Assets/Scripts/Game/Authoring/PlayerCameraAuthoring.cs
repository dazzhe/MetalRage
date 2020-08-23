using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerCameraAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponentData(entity, new PlayerCamera {
            ForwardOffsetFactor = 4.7f,
            LeanLength = 5f,
        });
        dstManager.AddComponentData(entity, new PlayerCameraCommand());
        dstManager.AddComponentData(entity, new CopyTransformToGameObject());
    }
}

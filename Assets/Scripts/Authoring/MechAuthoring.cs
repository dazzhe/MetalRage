using Unity.Entities;
using UnityEngine;

public class MechAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    [SerializeField]
    private MechType mechType;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        var mechConfigMap = Game.Config.GetConfig<MechConfigMap>();
        var mechConfig = mechConfigMap[this.mechType];
        dstManager.AddComponentData(entity, new MechMovementStatus());
        dstManager.AddComponentData(entity, new BoosterEngineStatus { Gauge = 100 });
        dstManager.AddComponentData(entity, new BoosterConfigData {
            MaxSpeed = 30f,
            Consumption = 1,
            Duration = 0.2f,
            Accel = 300f
        });
        dstManager.AddComponentData(entity, mechConfig.Movement);
        throw new System.NotImplementedException();
    }
}

using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct GameConfigData : ISharedComponentData {
    public Entity VanguardPrefabEntity;
}

[InternalBufferCapacity(8)]
public struct MechPrefabEntry : IBufferElementData {
    public MechType Type;
    public Entity Prefab;
}

public class GameConfigLoader : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs {
    [SerializeField]
    GameConfig config;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        var prefabEntity = conversionSystem.GetPrimaryEntity(this.config.GetConfig<MechConfigMap>()[MechType.Vanguard].Prefab);
        var configEntity = dstManager.CreateEntity();
        var buffer = dstManager.AddBuffer<MechPrefabEntry>(configEntity);
        buffer.Add(new MechPrefabEntry {
            Type = MechType.Vanguard,
            Prefab = prefabEntity
        });
        var mechConfig = this.config.GetConfig<MechConfigMap>()[MechType.Vanguard];
        dstManager.AddComponentData(prefabEntity, new MechMovementStatus());
        dstManager.AddComponentData(prefabEntity, new MechRequestedMovement());
        dstManager.AddComponentData(prefabEntity, new MechCommand());
        dstManager.AddComponentData(prefabEntity, new BoosterEngineStatus { Gauge = 100 });
        dstManager.AddComponentData(prefabEntity, mechConfig.HEngineConfig.Data);
        dstManager.AddComponentData(prefabEntity, mechConfig.Movement);
        dstManager.AddComponentData(prefabEntity, new CharacterRigidbody {
            GroundProbeVector = new float3(0f, -0.2f, 0f),
            MaxSlope = math.radians(60f),
            MaxIterations = 10,
            CharacterMass = 1f,
            SkinWidth = 0.04f,
            ContactTolerance = 0.2f,
            AffectsPhysicsBodies = 1,
            MaxMovementSpeed = 50f,
            CapsuleCenter = new float3(0f, 1.9f, 0f),
            CapsuleHeight = 3.8f,
            CapsuleRadius = 1f
        });
        dstManager.AddComponent(prefabEntity, typeof(CharacterPhysicsVelocity));
        dstManager.AddComponent(prefabEntity, typeof(CharacterPhysicsInput));
        dstManager.AddComponent(prefabEntity, typeof(CharacterPhysicsOutput));
        dstManager.AddComponent(prefabEntity, typeof(GroundContactStatus));

    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) {
        var mechPrefabs = this.config.GetConfig<MechConfigMap>().GetAllConfigs().Select(config => config.Prefab);
        referencedPrefabs.AddRange(mechPrefabs);
    }
}

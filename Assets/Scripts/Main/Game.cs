using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField]
    private GameConfig config = default;

    public static SoundSystem SoundSystem { get; private set; }
    public static GameConfig Config { get; private set; }
    public static Dictionary<MechType, Entity> MechPrefabMap = new Dictionary<MechType, Entity>();

    private void Awake() {
        SoundSystem = new SoundSystem();
        Config = this.config;
        //var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //var mechConfigMap = Config.GetConfig<MechConfigMap>();
        //var prefab = mechConfigMap[MechType.Vanguard].Prefab;

        //entityManager.AddComponentData(prefabEntity, new Prefab());
        //entityManager.AddComponentData(prefabEntity, new MechMovementStatus());
        //entityManager.AddComponentData(prefabEntity, new BoosterEngineStatus { Gauge = 100 });
        //entityManager.AddComponentData(prefabEntity, new BoosterConfigData {
        //    MaxSpeed = 30f,
        //    Consumption = 1,
        //    Duration = 0.2f,
        //    Accel = 300f
        //});
        //entityManager.AddComponentData(prefabEntity, mechConfigMap[MechType.Vanguard].Movement);
        //entityManager.AddComponentObject(prefabEntity, prefab.GetComponent<Transform>());
//        MechPrefabMap.Add(MechType.Vanguard, prefabEntity);
    }
}

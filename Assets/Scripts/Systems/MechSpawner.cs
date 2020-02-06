using Unity.Entities;
using UnityEngine;

public class MechSpawner : ComponentSystem {
    protected override void OnCreate() {
        base.OnCreate();
    }

    protected override void OnUpdate() {
        this.Entities.ForEach((Entity entity, ref MechSpawnRequest request) => {
            this.PostUpdateCommands.DestroyEntity(entity);
            var spawnedObject = GameObject.Instantiate(Game.Config.GetConfig<MechConfigMap>()[request.MechType].Prefab);
            var transform = spawnedObject.GetComponent<Transform>();
            transform.position = request.Position;
            transform.rotation = request.Rotation;
            var mechConfigMap = Game.Config.GetConfig<MechConfigMap>();
            var mechConfig = mechConfigMap[request.MechType];
            var spawnedEntity = this.EntityManager.CreateEntity();
            this.PostUpdateCommands.AddComponent(spawnedEntity, new MechMovementStatus());
            this.PostUpdateCommands.AddComponent(spawnedEntity, new BoosterEngineStatus { Gauge = 100 });
            this.PostUpdateCommands.AddComponent(spawnedEntity, new BoosterConfigData {
                MaxSpeed = 30f,
                Consumption = 1,
                Duration = 0.2f,
                Regeneration = 30f,
                Accel = 300f
            });
            this.PostUpdateCommands.AddComponent(spawnedEntity, mechConfig.Movement);
            this.EntityManager.AddComponentObject(spawnedEntity, transform);
            this.EntityManager.AddComponentObject(spawnedEntity, spawnedObject.GetComponent<CharacterController>());
            this.EntityManager.AddComponentObject(spawnedEntity, spawnedObject.GetComponent<Animator>());
            this.EntityManager.AddComponentObject(spawnedEntity, spawnedObject.GetComponent<MechComponent>());
            var mech = new Mech {
                CameraTargetTranslation = spawnedObject.GetComponent<MechAuthoring>().CameraTarget.position
            };
            this.PostUpdateCommands.AddComponent(spawnedEntity, mech);
        });
    }
}

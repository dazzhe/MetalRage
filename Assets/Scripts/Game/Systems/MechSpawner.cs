using Unity.Entities;
using Unity.Mathematics;
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
            this.PostUpdateCommands.AddComponent(spawnedEntity, new MechRequestedMovement());
            this.PostUpdateCommands.AddComponent(spawnedEntity, new MechCommand());
            this.PostUpdateCommands.AddComponent(spawnedEntity, new BoosterEngineStatus { Gauge = 100 });
            this.PostUpdateCommands.AddComponent(spawnedEntity, mechConfig.HEngineConfig.Data);
            this.PostUpdateCommands.AddComponent(spawnedEntity, mechConfig.Movement);
            this.EntityManager.AddComponentObject(spawnedEntity, transform);
            //this.EntityManager.AddComponentObject(spawnedEntity, spawnedObject.GetComponent<CharacterController>());
            this.PostUpdateCommands.AddComponent(entity, new CharacterRigidbody {
                GroundProbeVector = new float3(0f, -0.1f, 0f),
                MaxSlope = math.radians(60f),
                MaxIterations = 10,
                CharacterMass = 1f,
                SkinWidth = 0.02f,
                ContactTolerance = 0.1f,
                AffectsPhysicsBodies = 1,
                MaxMovementSpeed = 50f,
                CapsuleCenter = new float3(0f, 1f, 0f),
                CapsuleHeight = 2f,
                CapsuleRadius = 0.5f
            });
            this.PostUpdateCommands.AddComponent(entity, typeof(CharacterPhysicsVelocity));
            this.PostUpdateCommands.AddComponent(entity, typeof(CharacterPhysicsInput));
            this.PostUpdateCommands.AddComponent(entity, typeof(CharacterPhysicsOutput));
            this.PostUpdateCommands.AddComponent(entity, typeof(GroundContactStatus));
            this.EntityManager.AddComponentObject(spawnedEntity, spawnedObject.GetComponent<Animator>());
            this.EntityManager.AddComponentObject(spawnedEntity, spawnedObject.GetComponent<MechComponent>());
            var mech = new Mech {
                BaseCameraOffset = spawnedObject.GetComponent<MechAuthoring>().CameraTarget.localPosition
            };
            this.PostUpdateCommands.AddComponent(spawnedEntity, mech);
        });
    }
}

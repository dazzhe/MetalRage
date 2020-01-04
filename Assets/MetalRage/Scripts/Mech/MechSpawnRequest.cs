using Unity.Entities;
using UnityEngine;

public struct MechSpawnRequest : IComponentData {
    [SerializeField]
    private MechType mechType;
    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private Quaternion rotation;
    [SerializeField]
    private Entity playerEntity;

    public MechType MechType { get => this.mechType; set => this.mechType = value; }
    public Vector3 Position { get => this.position; set => this.position = value; }
    public Quaternion Rotation { get => this.rotation; set => this.rotation = value; }
    public Entity PlayerEntity { get => this.playerEntity; set => this.playerEntity = value; }

    private MechSpawnRequest(MechType mechType, Vector3 position, Quaternion rotation, Entity playerEntity) {
        this.mechType = mechType;
        this.position = position;
        this.rotation = rotation;
        this.playerEntity = playerEntity;
    }

    public static void Create(EntityCommandBuffer commandBuffer, MechType mechType, Vector3 position, Quaternion rotation, Entity playerEntity) {
        var data = new MechSpawnRequest(mechType, position, rotation, playerEntity);
        var entity = commandBuffer.CreateEntity();
        commandBuffer.AddComponent(entity, data);
    }
}

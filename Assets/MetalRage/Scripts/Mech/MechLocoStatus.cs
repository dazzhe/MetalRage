using Unity.Entities;
using UnityEngine;

public struct MechLocoStatus : IComponentData {
    [SerializeField]
    private byte isOnGround;
    [SerializeField]
    private MechLocoState state;
    [SerializeField]
    private Vector3 velocity;

    public bool IsOnGround { get => this.isOnGround != 0; set => this.isOnGround = value ? (byte)1 : (byte)0; }
    public MechLocoState State { get => this.state; set => this.state = value; }
    public Vector3 Velocity { get => this.velocity; set => this.velocity = value; }
}

public class MechLocoSystem : ComponentSystem {
    private ComponentGroup group;

    protected override void OnCreateManager() {
        base.OnCreateManager();
        this.group = GetComponentGroup(typeof(CharacterController), typeof(MechLocoStatus));
    }

    protected override void OnUpdate() {
        var characterControllers = this.group.GetComponentArray<CharacterController>();
        var locoStatuses = this.group.GetComponentDataArray<MechLocoStatus>();
        for (int i = 0; i < characterControllers.Length; ++i) {
            characterControllers[i].Move(locoStatuses[i].Velocity * Time.deltaTime);
        }
    }
}
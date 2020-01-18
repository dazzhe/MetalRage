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

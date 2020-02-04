using Unity.Entities;
using UnityEngine;

public struct MechMovementStatus : IComponentData {
    public Bool IsOnGround;
    public MechMovementState State;
    public Vector3 Velocity;
    public float LegYaw;
    public float Yaw;
    public float Pitch;
}

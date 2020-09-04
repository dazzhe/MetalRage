using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public enum MechMovementState {
    Stand,
    Walking,
    Crouching,
    Airborne,
    BoostAcceling,
    BoostBraking
}

public struct MechMovementStatus : IComponentData {
    public Bool IsOnGround;
    public MechMovementState State;
    public float3 Velocity;
    public float LegYaw;
    public float Yaw;
    public float Pitch;
}

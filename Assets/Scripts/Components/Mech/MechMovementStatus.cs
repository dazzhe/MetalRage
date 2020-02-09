using Unity.Entities;
using UnityEngine;
public enum MechMovementState {
    Idle,
    Walking,
    Crouching,
    Airborne,
    Boosting,
    Braking
}

public struct MechMovementStatus : IComponentData {
    public MechActionMask availableActionMask;
    public MechActionMask activatableActionMask;
    public Bool IsOnGround;
    public MechMovementState State;
    public Vector3 Velocity;
    public float LegYaw;
    public float Yaw;
    public float Pitch;
}

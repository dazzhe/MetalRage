using Unity.Entities;
using UnityEngine;

public struct MechRequestedMovement {
    public Vector3 Motion;
    public MechMovementState State;
    public float LegYaw;
    public BlittableBool IsLeavingGround;
}

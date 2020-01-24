using Unity.Entities;
using UnityEngine;

public struct MechLocoStatus : IComponentData {
    public BlittableBool IsOnGround;
    public MechLocoState State;
    public Vector3 Velocity;
    public float LegYaw;
}

using Unity.Entities;
using Unity.Mathematics;

public struct MechRequestedMovement : IComponentData {
    public float3 Velocity;
    public MechMovementState State;
    public float LegYaw;
    public Bool UseRawVelocity;
}
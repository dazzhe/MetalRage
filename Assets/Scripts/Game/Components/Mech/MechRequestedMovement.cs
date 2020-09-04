using Unity.Entities;
using Unity.Mathematics;

public struct MechRequestedMovement : IComponentData {
    public MechMovementState State;
    public float LegYaw;
    public float3 Velocity;
    public Bool ShouldJump;
}
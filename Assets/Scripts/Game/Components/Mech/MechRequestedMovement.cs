using Unity.Entities;
using Unity.Mathematics;

public struct MechRequestedMovement : IComponentData {
    public float3 Velocity;
    public MechMovementState State;
    public float LegYaw;
    /// <summary>
    /// Whether or not the mech should follow ground if it is on ground.
    /// This value does not affect anything if the mech is in air.
    /// </summary>
    public Bool ShouldFollowGround;
    public Bool ShouldApplyGravity;
}
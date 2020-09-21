using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct MechMovementConfigData : IComponentData {
    public float MinPitch;
    public float MaxPitch;
    public float MaxMovementSpeedInAir;
    public float MaxFallSpeed;
    public float Gravity;
    public float MaxSlopeAngle;
    [Header("Walk")]
    public float WalkingAcceleration;
    public float WalkingDeceleration;
    public float MaxWalkSpeed;
    [Header("Jump")]
    public float BaseJumpSpeed;
}
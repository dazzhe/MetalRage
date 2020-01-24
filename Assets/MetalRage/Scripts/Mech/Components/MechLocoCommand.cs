using Unity.Entities;
using UnityEngine;

public struct MechLocoCommand : IComponentData {
    public Vector3 Motion;
    public MechLocoState NextState;
    public float LegYaw;
}

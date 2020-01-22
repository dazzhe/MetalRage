using Unity.Entities;
using UnityEngine;

public struct MechLocoCommand : IComponentData {
    [SerializeField]
    private Vector3 motion;
    [SerializeField]
    private MechLocoState nextState;
    [SerializeField]
    private float legYaw;

    public Vector3 Motion { get => this.motion; set => this.motion = value; }
    public MechLocoState NextState { get => this.nextState; set => this.nextState = value; }
    public float LegYaw { get => this.legYaw; set => this.legYaw = value; }
}

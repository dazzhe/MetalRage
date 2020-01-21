using Unity.Entities;
using UnityEngine;

public struct MechLocoCommand : IComponentData {
    [SerializeField]
    private Vector3 motion;
    [SerializeField]
    private MechLocoState nextState;

    public Vector3 Motion { get => this.motion; set => this.motion = value; }
    public MechLocoState NextState { get => this.nextState; set => this.nextState = value; }
}

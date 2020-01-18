using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct WalkActionConfigData : IComponentData {
    [SerializeField]
    private float accel;
    [SerializeField]
    private float decel;
    [SerializeField]
    private float maxSpeed;

    public float Accel { get => this.accel; set => this.accel = value; }
    public float Decel { get => this.decel; set => this.decel = value; }
    public float MaxSpeed { get => this.maxSpeed; set => this.maxSpeed = value; }
}

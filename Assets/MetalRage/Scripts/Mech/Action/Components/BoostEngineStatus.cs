using Unity.Entities;
using UnityEngine;

public struct BoostEngineStatus : IComponentData {
    [SerializeField]
    private float gauge;

    public float Gauge { get => this.gauge; set => this.gauge = value; }
}

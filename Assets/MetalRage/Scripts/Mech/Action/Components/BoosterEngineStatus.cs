using Unity.Entities;
using UnityEngine;

public struct BoosterEngineStatus : IComponentData {
    [SerializeField]
    private float gauge;
    [SerializeField]
    private float elapsedTime;

    public float Gauge { get => this.gauge; set => this.gauge = value; }
    public float ElapsedTime { get => this.elapsedTime; set => this.elapsedTime = value; }
}

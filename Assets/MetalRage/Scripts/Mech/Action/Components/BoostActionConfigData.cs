using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct BoostActionConfigData : IComponentData {
    [SerializeField]
    private float regeneration;
    [SerializeField]
    private float consumption;
    [SerializeField]
    private float cooldownTime;
    [SerializeField]
    private float accel;
    [SerializeField]
    private float decel;
    [SerializeField]
    private float maxSpeed;

    private const float kMaxGauge = 100f;

    public float MaxGauge => kMaxGauge;
    public float Accel { get => this.accel; set => this.accel = value; }
    public float Decel { get => this.decel; set => this.decel = value; }
    public float MaxSpeed { get => this.maxSpeed; set => this.maxSpeed = value; }
    public float Regeneration { get => this.regeneration; set => this.regeneration = value; }
    public float Consumption { get => this.consumption; set => this.consumption = value; }
    public float CooldownTime { get => this.cooldownTime; set => this.cooldownTime = value; }
}

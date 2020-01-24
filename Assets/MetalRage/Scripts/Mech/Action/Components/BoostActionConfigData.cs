using Unity.Entities;

[System.Serializable]
public struct BoostActionConfigData : IComponentData {
    public static readonly float MaxGauge = 100f;
    public float Regeneration;
    public float Consumption;
    public float CooldownTime;
    public float Accel;
    public float Decel;
    public float MaxSpeed;
}

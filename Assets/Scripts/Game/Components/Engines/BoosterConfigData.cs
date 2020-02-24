using Unity.Entities;

[System.Serializable]
public struct BoosterConfigData : IComponentData {
    public static readonly float MaxGauge = 100f;
    public float Duration;
    public float Regeneration;
    public float Consumption;
    public float Accel;
    public float MaxSpeed;
    public float BrakingDeceleration;
}

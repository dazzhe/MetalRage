using Unity.Entities;

[System.Serializable]
public struct WalkActionConfigData : IComponentData {
    public float Accel;
    public float Decel;
    public float MaxSpeed;
}

using Unity.Entities;

public struct BoosterEngineStatus
    : IComponentData {
    public float Gauge;
    public float ElapsedTime;
}

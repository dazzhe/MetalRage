using Unity.Entities;
using UnityEngine;

public class BoosterEngineSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref BoosterEngineStatus status, ref BoosterConfigData config) => {
            status.Gauge += config.Regeneration * this.Time.DeltaTime;
            status.Gauge = Mathf.Min(status.Gauge, BoosterConfigData.MaxGauge);
        });
    }
}

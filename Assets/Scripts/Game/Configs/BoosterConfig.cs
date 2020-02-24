using UnityEngine;

[CreateAssetMenu(fileName = "BoosterConfig", menuName = "MetalRage/Engine/Config")]
public class BoosterConfig : ScriptableObject {
    [SerializeField]
    private BoosterConfigData data = new BoosterConfigData {
        MaxSpeed = 40f,
        Consumption = 1,
        Duration = 0.2f,
        Regeneration = 30f,
        Accel = 300f,
        BrakingDeceleration = 10f,
    };

    public BoosterConfigData Data { get => this.data; set => this.data = value; }
}

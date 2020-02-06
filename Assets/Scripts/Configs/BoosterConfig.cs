using UnityEngine;

[CreateAssetMenu(fileName = "BoosterConfig", menuName = "MetalRage/Engine/Config")]
public class BoosterConfig : ScriptableObject {
    [SerializeField]
    private BoosterConfigData data = new BoosterConfigData {
        Duration = 0.2f,
        Regeneration = 30f,
        Consumption = 28f,
        Accel = 300f
    };

    public BoosterConfigData Data { get => this.data; set => this.data = value; }
}

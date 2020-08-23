using UnityEngine;

[CreateAssetMenu(fileName = "MechConfig", menuName = "MetalRage/Mech/Config")]
public class MechConfig : ScriptableObject {
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private MechMovementConfigData movement = new MechMovementConfigData {
        MaxWalkSpeed = 13f,
        WalkingAcceleration = 60f,
        WalkingDeceleration = 50f,
        BaseJumpSpeed = 30f,
        Gravity = -40f,
        MinPitch = -60f,
        MaxPitch = 60f,
        MaxSlopeAngle = 45f,
        MaxMovementSpeedInAir = 20f,
        MaxFallSpeed = 50f
    };
    [SerializeField]
    private BoosterConfig hEngineConfig;

    public GameObject Prefab { get => this.prefab; set => this.prefab = value; }
    public MechMovementConfigData Movement { get => this.movement; set => this.movement = value; }
    public BoosterConfig HEngineConfig { get => this.hEngineConfig; set => this.hEngineConfig = value; }
}

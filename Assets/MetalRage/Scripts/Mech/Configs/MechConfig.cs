using UnityEngine;

[CreateAssetMenu(fileName = "MechConfig", menuName = "MetalRage/Mech/Config")]
public class MechConfig : ScriptableObject {
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private MechMovementConfigData movement = new MechMovementConfigData {
        MaxWalkSpeed = 13f,
        WalkAccel = 60f,
        Decel = 50f,
        BaseJumpSpeed = 30f
    };

    public GameObject Prefab { get => this.prefab; set => this.prefab = value; }
    public MechMovementConfigData Movement { get => this.movement; set => this.movement = value; }
}

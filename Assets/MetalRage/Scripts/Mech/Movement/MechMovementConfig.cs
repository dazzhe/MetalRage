using UnityEngine;

[CreateAssetMenu(fileName = "New MechMovementConfig.asset", menuName = "MetalRage/Mech/MovementConfig")]
public class MechMovementConfig : ScriptableObject {
    [SerializeField]
    private MechMovementConfigData data;

    public MechMovementConfigData Data { get => this.data; set => this.data = value; }
}

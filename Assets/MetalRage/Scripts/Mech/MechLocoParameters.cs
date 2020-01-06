using UnityEngine;

[CreateAssetMenu(fileName = "MechLocoParameters", menuName = "MetalRage/Mech/MechLocoParameters")]
public class MechLocoParameters : ScriptableObject {
    [SerializeField]
    private bool canBoost = true;
    [SerializeField]
    private float maxBoostGauge = 100f;
    [SerializeField]
    private float boostSpeed = 40f;
    [SerializeField]
    private float boostDuration = 0.2f;
    [SerializeField]
    private float jumpSpeed = 18f;
    [SerializeField]
    private float horizontalJumpSpeed = 20f;
    [SerializeField]
    private float walkSpeed = 17f;
    [SerializeField]
    private float accelSpeed = 0.2f;

    public bool CanBoost { get => this.canBoost; set => this.canBoost = value; }
    public float BoostSpeed { get => this.boostSpeed; set => this.boostSpeed = value; }
    public float JumpSpeed { get => this.jumpSpeed; set => this.jumpSpeed = value; }
    public float WalkSpeed { get => this.walkSpeed; set => this.walkSpeed = value; }
    public float AccelSpeed { get => this.accelSpeed; set => this.accelSpeed = value; }
    public float MaxBoostGauge { get => this.maxBoostGauge; set => this.maxBoostGauge = value; }
    public float BoostDuration { get => this.boostDuration; set => this.boostDuration = value; }
    public float HorizontalJumpSpeed { get => this.horizontalJumpSpeed; set => this.horizontalJumpSpeed = value; }
}

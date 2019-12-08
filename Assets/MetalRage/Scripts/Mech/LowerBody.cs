using UnityEngine;

public class LowerBody : MonoBehaviour {
    [SerializeField]
    private GameObject unit = default;
    private UnitMotor motor;

    private Animator animator;

    private void Start() {
        this.motor = this.unit.GetComponent<UnitMotor>();
        this.animator = GetComponent<Animator>();
    }

    private void Update() {
        this.animator.SetBool("IsGrounded", this.motor.isGrounded);
        if (this.motor.locoState == MechLocoState.Walking) {
            this.animator.SetFloat("WalkSpeed", this.motor.MoveDirection.magnitude);
        }
        if (this.motor.locoState == MechLocoState.Idle) {
            this.animator.SetFloat("WalkSpeed", this.motor.MoveDirection.magnitude);
        }
        this.animator.SetBool("IsCrouching", this.motor.locoState == MechLocoState.Crouching);
    }

    // Call from animation clip.
    private void PlayWalkClip() {
        GetComponent<AudioSource>().Play();
    }
}
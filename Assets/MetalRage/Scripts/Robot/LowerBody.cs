using UnityEngine;

public class LowerBody : MonoBehaviour {
    [SerializeField]
    private GameObject unit;
    private UnitMotor motor;

    private Animator animator;

    private void Start() {
        this.motor = this.unit.GetComponent<UnitMotor>();
        this.animator = GetComponent<Animator>();
    }

    private void Update() {
        this.animator.SetBool("IsGrounded", this.motor.isGrounded);
        if (this.motor.characterState == RobotState.Walking) {
            this.animator.SetFloat("WalkSpeed", this.motor.MoveDirection.magnitude);
        }
        if (this.motor.characterState == RobotState.Idle) {
            this.animator.SetFloat("WalkSpeed", this.motor.MoveDirection.magnitude);
        }
        this.animator.SetBool("IsCrouching", this.motor.characterState == RobotState.Crouching);
    }

    // Call from animation clip.
    private void PlayWalkClip() {
        GetComponent<AudioSource>().Play();
    }
}
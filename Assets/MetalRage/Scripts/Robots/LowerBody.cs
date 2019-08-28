using UnityEngine;

public class LowerBody : MonoBehaviour {
    [SerializeField]
    private GameObject unit;
    private UnitMotor motor;

    private new Animation animation;

    private void Start() {
        this.motor = this.unit.GetComponent<UnitMotor>();
        this.animation = GetComponent<Animation>();
    }

    private void Update() {
        if (this.motor._characterState == UnitMotor.CharacterState.Walking) {
            this.animation.CrossFade("Walk", 0.1f);
        } else if (this.animation.IsPlaying("Walk")) {
            this.animation.Stop("Walk");
        }
        if (this.motor._characterState == UnitMotor.CharacterState.Idle) {
            this.animation.CrossFade("Idle", 0.5f);
        }
        if (this.motor._characterState == UnitMotor.CharacterState.Squatting) {
            this.animation.CrossFade("Squat", 0.1f);
        }
    }

    // Call from animation clip.
    private void PlayWalkClip() {
        GetComponent<AudioSource>().Play();
    }
}
using UnityEngine;
using System.Collections;

public class LowerBody : MonoBehaviour {
	public GameObject van;

	UnitMotor motor;


	private Animation _animation;

	void Start () {
		motor = van.GetComponent<UnitMotor>();
		_animation = GetComponent<Animation>();
		//animation["Walk"].speed = 1.2f;
	}

	void Update () {
		if (motor._characterState == UnitMotor.CharacterState.Walking)
			_animation.CrossFade ("Walk",0.1f);
		else if (_animation.IsPlaying("Walk"))
			_animation.Stop("Walk");

		if (motor._characterState == UnitMotor.CharacterState.Idle){
			_animation.CrossFade ("Idle",0.5f);
		}

		/*if (vanguard._characterState == Vanguard.CharacterState.Boosting){
			if (vanguard.boosttype == 0){}
				_animation.Play ("boost_right");
			if (vanguard.boosttype == 1){}
				_animation.Play ("boost");
			if (vanguard.boosttype == 2){}
				_animation.Play ("boost_left");
		}*/

		//if (vanguard.Ground && Input.GetButtonDown("Jump"))
			//_animation.Play("jump_2");

		if (motor._characterState == UnitMotor.CharacterState.Squatting)
			_animation.CrossFade("Squat",0.1f);

	}

	void play(){
		audio.Play();
	}
}
using UnityEngine;
using System.Collections;

public class LegDirection : MonoBehaviour {
	[SerializeField]
	private GameObject unit;
	
	UnitMotor motor;
	private float h;
	private float v;
	private static Vector3 rf =
		new Vector3(1, 0, 1);
	private static Vector3 lf =
		new Vector3(-1, 0, 1);

	void Start () {
		motor = unit.GetComponent<UnitMotor>();
	}

	void Update () {
		if (motor._characterState == UnitMotor.CharacterState.Walking){
			h = motor.inputMoveDirection.x;
			v = motor.inputMoveDirection.y;
			if (h > 0 && v > 0)
				transform.rotation = Quaternion.Slerp(transform.rotation,
				                                      Quaternion.LookRotation(unit.transform.TransformDirection(rf)), 
				                                      0.1f);
			else if (h > 0 && v == 0)
				transform.rotation = Quaternion.Slerp(transform.rotation,
				                                      Quaternion.LookRotation(unit.transform.right),
				                                      0.1f);
			else if ((h == 0 && v > 0) || v < 0)
				transform.rotation = Quaternion.Slerp(transform.rotation,
				                                      Quaternion.LookRotation(unit.transform.forward),
				                                      0.1f);
			else if (h < 0 && v > 0)
				transform.rotation = Quaternion.Slerp(transform.rotation,
				                                      Quaternion.LookRotation(unit.transform.TransformDirection(lf)), 
				                                      0.1f);
			else if (h < 0 && v == 0)
				transform.rotation = Quaternion.Slerp(transform.rotation,
				                                      Quaternion.LookRotation(-unit.transform.right), 
				                                      0.1f);
		}
		if (motor._characterState == UnitMotor.CharacterState.Boosting){
			transform.rotation = Quaternion.Slerp(transform.rotation,
			                                      Quaternion.LookRotation(unit.transform.forward),
			                                      0.1f);
		}
	}
}

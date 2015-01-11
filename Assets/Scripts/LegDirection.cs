using UnityEngine;
using System.Collections;

public class LegDirection : MonoBehaviour {
	public GameObject van;
	
	UnitMotor motor;
	private float h;
	private float v;
	private static Vector3 rf =
		new Vector3(1, 0, 1);
	private static Vector3 lf =
		new Vector3(-1, 0, 1);
	// Use this for initialization
	void Start () {
		motor = van.GetComponent<UnitMotor>();
	}
	
	// Update is called once per frame
	void Update () {
		if (motor._characterState == UnitMotor.CharacterState.Walking){
			h = Input.GetAxisRaw("Horizontal");
			v = Input.GetAxisRaw("Vertical");
			if (h > 0 && v > 0)
				transform.rotation = Quaternion.Slerp(transform.rotation,
				                                      Quaternion.LookRotation(van.transform.TransformDirection(rf)), 
				                                      0.1f);
			else if (h > 0 && v == 0)
				transform.rotation = Quaternion.Slerp(transform.rotation,
				                                      Quaternion.LookRotation(van.transform.right),
				                                      0.1f);
			else if ((h == 0 && v > 0) || v < 0)
				transform.rotation = Quaternion.Slerp(transform.rotation,
				                                      Quaternion.LookRotation(van.transform.forward),
				                                      0.1f);
			else if (h < 0 && v > 0)
				transform.rotation = Quaternion.Slerp(transform.rotation,
				                                      Quaternion.LookRotation(van.transform.TransformDirection(lf)), 
				                                      0.1f);
			else if (h < 0 && v == 0)
				transform.rotation = Quaternion.Slerp(transform.rotation,
				                                      Quaternion.LookRotation(-van.transform.right), 
				                                      0.1f);
		}
		if (motor._characterState == UnitMotor.CharacterState.Boosting){
			transform.rotation = Quaternion.Slerp(transform.rotation,
			                                      Quaternion.LookRotation(van.transform.forward),
			                                      0.1f);
		}
	}
}

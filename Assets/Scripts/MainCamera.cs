using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {
	private AudioSource lean;
	public GameObject objc;
	WeaponControl weaponctrl;
	UnitMotor motor;
	public Transform cameraTransform;
	private float positionz;
	public Vector3 cp;
	private Vector3 defaultposition;
	private Vector3 oldfd;
	private Vector3 position;
	private Vector3 desiredPosition;
	private Vector3 lposition;
	private Vector3 ldefaultposition;
	private Vector3 closeOffset = new Vector3 (0f, 2.5f, -1f);
	private float offset = 0;
	private bool leaning;
	private int dir;

	void Start () {
		AudioSource[] audioSources = GetComponents<AudioSource>();
		lean = audioSources[1];
		leaning = false;
		weaponctrl = GetComponent<WeaponControl>();
		motor = GetComponent<UnitMotor>();
		oldfd = objc.transform.forward;
	}

	void OnEnable()
	{
		if (!cameraTransform && Camera.main)
			cameraTransform = Camera.main.transform;
		if (!cameraTransform){
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Setting ();
		Direction();
		Lean();
		if (!leaning){
			Follow();
			Distance ();
		//transform.position = cp + transform.right * positionx
		//	+ transform.forward * positionz;
		} else {
			offset = Mathf.Lerp(offset, dir * 5f, 0.2F);
			position = defaultposition + transform.right * offset;
		}
		Vector3 closePosition = transform.position + transform.rotation * closeOffset;
		RaycastHit hit;
		Vector3 closeToFar = position - closePosition;
		if (Physics.Raycast(closePosition, closeToFar, out hit, closeToFar.magnitude + 0.03f)) 
			desiredPosition = hit.point;
		else
			desiredPosition = position;
		cameraTransform.position = desiredPosition;
	}

	void Setting(){
		cp = transform.position;
		cp.y += 2.5F;
		defaultposition = cp + positionz * objc.transform.forward;
	}

	void Lean(){
		if (motor.moveDirection.x == 0 && motor.moveDirection.z == 0){
			if (Input.GetButtonDown ("right lean")){
				lean.PlayOneShot (lean.clip);
				leaning = true;
				dir = 1;
			}
			if (Input.GetButtonDown ("left lean")){
				lean.PlayOneShot (lean.clip);
				leaning = true;
				dir = -1;
			}
		}
		if (!Input.GetButton("right lean") && !Input.GetButton("left lean") && leaning){
			leaning = false;
			offset = 0;
		}
	}

	void Direction(){
		//olddirection = transform.rotation;
		cameraTransform.rotation = objc.transform.rotation;
		position = cp + Quaternion.FromToRotation(oldfd, objc.transform.forward) * (position - cp);
		oldfd = objc.transform.forward;
		//position = transform.position + transform.right * Vector3.Dot(transform.right, position - transform.position);
	}
	
	void Distance(){
		positionz = -5.5F + 4F * Mathf.Abs(weaponctrl.rotationY) / 60F;
	}
	
	void Follow(){
		lposition = transform.InverseTransformDirection(position);
		ldefaultposition = transform.InverseTransformDirection(defaultposition);
		lposition.x = Mathf.Lerp(lposition.x, ldefaultposition.x, 7F * Time.deltaTime);
		lposition.y = Mathf.Lerp(lposition.y, ldefaultposition.y, 20F * Time.deltaTime);
		lposition.z = Mathf.Lerp(lposition.z, ldefaultposition.z, 20F * Time.deltaTime);
		position = transform.TransformDirection(lposition);
		defaultposition = transform.TransformDirection(ldefaultposition);
	}
}
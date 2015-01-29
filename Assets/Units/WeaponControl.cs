using UnityEngine;
using System.Collections;

public class WeaponControl : MonoBehaviour {
	UnitMotor motor;
	Canvas settings;
	GameObject aimedObject;
	GameObject hinderingObject;
	public GameObject targetObject;

	public bool isRecoiling = false;
	[System.NonSerialized]
	public float rotationY = 0F;
	[System.NonSerialized]
	public float normalrotationY = 0F;
	[System.NonSerialized]
	public float recoilrotationx = 0F;
	[System.NonSerialized]
	public float recoilrotationy = 0F;

	[System.NonSerialized]
	public float dispersionRate;
	[System.NonSerialized]
	public float desiredDispersion;

	[System.NonSerialized]
	public Vector3 targetPos;
	[System.NonSerialized]
	public Vector3 shootDirection;

	[System.NonSerialized]
	public int load;

	private float minimumY = -60F;
	private float maximumY = 60F;
	public LayerMask mask;
	private Vector3 originPos;
	private float fixSpeed = 25f;
	public Vector3 center = Vector3.zero;
	public bool inputReload = false;
	public bool inputShot1 = false;
	[System.NonSerialized]
	public bool inputShot2 = false;
	Ray ray;
	RaycastHit hit;
	RaycastHit hinderinghit;
	
	private float DispersionCorrection(){
		switch(motor._characterState){
		case UnitMotor.CharacterState.Idle:
			return 1f;
		case UnitMotor.CharacterState.Walking:
			return 1f;
		case UnitMotor.CharacterState.Boosting:
			return 1.3f;
		case UnitMotor.CharacterState.Braking:
			return 1.2f;
		case UnitMotor.CharacterState.Squatting:
			return 0.5f;
		case UnitMotor.CharacterState.Jumping:
			return 1.5f;
		}
		return 1f;
	}
	
	void Start () {
		settings = GameObject.Find ("Settings").GetComponent<Canvas>();
		motor = GetComponent<UnitMotor>();
		center = new Vector3(Screen.width/2, Screen.height/2, 0);
		//8番のレイヤー(操作している機体)を無視する.
		mask = 1 << 8;
		mask = ~mask;
	}
	
	// Update is called once per frame
	void Update () {
		if (!settings.enabled){
			inputReload = Input.GetButtonDown("Reload");
			inputShot1 = Input.GetButton("Fire1");
			inputShot2 = Input.GetButtonDown("Fire2");
			Elevation ();
		} else {
			inputReload = false;
			inputShot1 = false;
			inputShot2 = false;
		}
		RecoilControl();
		DispersionControl();
	}

	void Elevation(){
		normalrotationY += Input.GetAxis ("Mouse Y") * Configuration.sensitivity * motor.sensimag;
		normalrotationY = Mathf.Clamp (normalrotationY, minimumY, maximumY);
		rotationY = normalrotationY + recoilrotationy;
	}
	
	void RecoilControl(){
		if ((recoilrotationx == 0 && recoilrotationy == 0) || isRecoiling)
			return;
		if (recoilrotationy > 0F)
			recoilrotationy -= fixSpeed * Time.deltaTime;
		if (recoilrotationy < 0f)
			recoilrotationy = 0f;
		if (recoilrotationx != 0)
			recoilrotationx = Mathf.Lerp(recoilrotationx,0,0.1F);
	}

	void DispersionControl(){
		if (dispersionRate > 1){
			dispersionRate -= 3f * Time.deltaTime;
		}
		if (dispersionRate < 1){
			dispersionRate = 1;
		}
		desiredDispersion = dispersionRate * DispersionCorrection();
	}

	void SetTarget(){
		if (Physics.Raycast(ray, out hit, 1000, mask)) {
			targetPos = hit.point;
			targetObject = hit.collider.gameObject;
		}
	}

	void OnDestroy(){
		NormalDisplay.WhiteReticle();
	}
}
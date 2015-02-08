using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponControl : MonoBehaviour {
	static Text EIR;
	UnitMotor motor;
	Canvas settings;
	WeaponSelect[] weapons = new WeaponSelect[3];
	private int enabledWeapon = 0;
	[System.NonSerialized]
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
	public float dispersionRate = 0;
	[System.NonSerialized]
	public float desiredDispersion;

	[System.NonSerialized]
	public Vector3 targetPos;

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
	public bool isBlitzMain = false;
	[System.NonSerialized]
	public bool inputShot2 = false;
	Ray ray;
	RaycastHit hit;

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
		EIR = GameObject.Find("NormalDisplay/EnemyInReticle").GetComponent<Text>();
		motor = GetComponent<UnitMotor>();
		weapons[0] = transform.Find ("Offset/MainWeapon").GetComponent<WeaponSelect>();
		weapons[1] = transform.Find ("Offset/RightWeapon").GetComponent<WeaponSelect>();
		weapons[2] = transform.Find ("Offset/LeftWeapon").GetComponent<WeaponSelect>();
		for (int i = 0; i <= 2; i++)
			weapons[i].enabled = true;
		weapons[0].SendEnable();
		center = new Vector3(Screen.width/2, Screen.height/2, 0);
		//8番のレイヤー(操作している機体)を無視する.
		mask = 1 << 8;
		mask = ~mask;
	}

	void Update () {
		if (!settings.enabled){
			WeaponSelect();
			inputReload = Input.GetButtonDown("Reload");
			inputShot1 = Input.GetButton("Fire1");
			inputShot2 = Input.GetButtonDown("Fire2");
			Elevation ();
		} else {
			inputReload = false;
			inputShot1 = false;
			inputShot2 = false;
		}
		if (!isBlitzMain)
			SetTarget();
		RecoilControl();
		DispersionControl();
	}

	void WeaponSelect(){
		if (Input.GetButtonDown("MainWeapon") && enabledWeapon != 0){
			weapons[enabledWeapon].SendDisable();
			weapons[0].SendEnable();
			enabledWeapon = 0;
		}
		if (Input.GetButtonDown("RightWeapon") && enabledWeapon != 1){
			weapons[enabledWeapon].SendDisable();
			weapons[1].SendEnable();
			enabledWeapon = 1;
		}
		if (Input.GetButtonDown("LeftWeapon") && enabledWeapon != 2){
			weapons[enabledWeapon].SendDisable();
			weapons[2].SendEnable();
			enabledWeapon = 2;
		}
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
		desiredDispersion = dispersionRate * DispersionCorrection() * Screen.height * 0.001f;
	}

	void SetTarget(){
		ray = Camera.main.ScreenPointToRay(center);
		if (Physics.Raycast(ray, out hit, 1000, mask)) {
			targetPos = hit.point;
			targetObject = hit.collider.gameObject;
			if (targetObject.layer == 10){
				StopCoroutine("ShowEnemyName");
				StartCoroutine("ShowEnemyName");
			}
		}
	}

	public void HitMark(){
		StopCoroutine("HitMarkCoroutine");
		StartCoroutine("HitMarkCoroutine");
	}
	
	private IEnumerator HitMarkCoroutine(){
		NormalDisplay.RedReticle();
		yield return new WaitForSeconds(0.3f);
		NormalDisplay.WhiteReticle();
	}
	private IEnumerator ShowEnemyName(){
		EIR.text = targetObject.GetComponentInParent<FriendOrEnemy>().playerName;
		yield return new WaitForSeconds(0.2f);
		EIR.text = "";
	}

	void OnDestroy(){
		NormalDisplay.WhiteReticle();
		EIR.text = "";
	}
}
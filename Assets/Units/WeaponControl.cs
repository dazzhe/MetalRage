using UnityEngine;
using System.Collections;

public class WeaponControl : MonoBehaviour {
	private AudioSource reload;
	public GameObject weapon1;
	public GameObject weapon2;
	UnitMotor motor;
	Canvas settings;
	GameObject aimedObject;
	GameObject hinderingObject;
	GameObject targetObject;

	//機体ごとにインスペクタ上で設定する.
	public int damage = 13;
	public float recoil = 0.4f;//反動.
	public float mindispersion = 10f;//ばらつき.
	public float dispersiongrow = 4f;
	public float maxrange = 0;
	public float reloadTime = 1.5f;
	public int maxload = 80;
	public float interval = 0.06F;

	[System.NonSerialized]
	public bool isReloading;
	[System.NonSerialized]
	public bool canShot;

	[System.NonSerialized]
	public float rotationY = 0F;
	[System.NonSerialized]
	public float normalrotationY = 0F;
	[System.NonSerialized]
	public float recoilrotationx = 0F;
	[System.NonSerialized]
	public float recoilrotationy = 0F;

	[System.NonSerialized]
	public float dispersion;
	[System.NonSerialized]
	public float desiredDispersion;

	[System.NonSerialized]
	public Vector3 target;
	[System.NonSerialized]
	public Vector3 shootDirection;

	[System.NonSerialized]
	public int load;

	private float minimumY = -60F;
	private float maximumY = 60F;
	private bool cooldown = false;
	private LayerMask mask;
	private Vector3 originPos;
	private float fixSpeed = 25f;
	private Vector3 center = Vector3.zero;
	private bool inputReload = false;
	private bool inputShot1 = false;
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
		AudioSource[] audioSources = GetComponents<AudioSource>();
		reload = audioSources[2];
		center = new Vector3(Screen.width/2, Screen.height/2, 0);
		load = maxload;
		canShot = true;
		dispersion = mindispersion;
		NormalDisplay.SetReticle(dispersion);
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
		StartCoroutine(ShotControl ());
		RecoilControl();
		DispersionControl();
		if (inputReload && load != maxload && !isReloading)
		    StartCoroutine(this.Reload());
	}

	void Elevation(){
		normalrotationY += Input.GetAxis ("Mouse Y") * Configuration.sensitivity * motor.sensimag;
		normalrotationY = Mathf.Clamp (normalrotationY, minimumY, maximumY);
		rotationY = normalrotationY + recoilrotationy;
	}

	IEnumerator ShotControl(){
		if (inputShot1 && load > 0 && !cooldown && canShot && !isReloading){
			Shot();
			MakeShots ();
			cooldown = true;
			yield return new WaitForSeconds(interval);
			cooldown = false;
		}
	}
	
	void MakeShots(){
		PhotonView weapon1PV = weapon1.GetComponent<PhotonView>();
		weapon1PV.RPC("MakeShot",PhotonTargets.All);
		PhotonView weapon2PV = weapon2.GetComponent<PhotonView>();
		weapon2PV.RPC("MakeShot",PhotonTargets.All);
	}

	void RecoilControl(){
		if ((recoilrotationx == 0 && recoilrotationy == 0) || cooldown)
			return;
		if (recoilrotationy > 0F)
			recoilrotationy -= fixSpeed * Time.deltaTime;
		if (recoilrotationy < 0f)
			recoilrotationy = 0f;
		if (recoilrotationx != 0)
			recoilrotationx = Mathf.Lerp(recoilrotationx,0,0.1F);
	}

	void DispersionControl(){
		if (dispersion > mindispersion){
			dispersion -= 30f * Time.deltaTime;
		}
		if (dispersion < mindispersion){
			dispersion = mindispersion;
		}
		desiredDispersion = dispersion * DispersionCorrection();
		NormalDisplay.SetReticle(desiredDispersion);
	}

	void Shot(){
		float r = Random.value * desiredDispersion;
		float theta = Random.value * 2 * Mathf.PI;
		Vector3 shootpoint = center + new Vector3(r * Mathf.Cos (theta), r * Mathf.Sin (theta), 0);
		ray = Camera.main.ScreenPointToRay(shootpoint);
		if (Physics.Raycast(ray, out hit, maxrange, mask)) {
			target = hit.point;
			aimedObject = hit.collider.gameObject;
			targetObject = aimedObject;

			//狙っているオブジェクトと機体の間に障害物があった場合.
			//障害物で弾がヒットするようにする.
			originPos = transform.position;
			shootDirection = (target - originPos).normalized;
			if (Physics.Raycast (originPos, shootDirection, out hinderinghit, maxrange, mask)){
			    targetObject = hinderinghit.collider.gameObject;
				target = hinderinghit.point;
			}

			//相手にダメージ判定があればレティクルを赤くし.
			//ダメージを与える.
			Hit dam = targetObject.GetComponentInParent<Hit>();
			if (dam != null){
				dam.TakeDamage(damage, this.gameObject);
				StopCoroutine("HitMark");
				StartCoroutine("HitMark");
			}
		} else {
			aimedObject = null;
			target = ray.GetPoint(maxrange);
		}
		RecoilAndDisperse ();
		RemainingLoads();
	}

	void RecoilAndDisperse(){
		if (recoilrotationy < 14F)
			recoilrotationy += recoil * (1f + dispersion * 0.1f);
		else
			recoilrotationy -= 1F;
		recoilrotationx += Random.Range(-recoil,recoil);
		if (dispersion < 30f)
			dispersion += dispersiongrow;
		else
			dispersion = 30f;
	}

	IEnumerator HitMark(){
		NormalDisplay.RedReticle();
		yield return new WaitForSeconds(0.3f);
		NormalDisplay.WhiteReticle();
	}

	void OnDestroy(){
		NormalDisplay.WhiteReticle();
	}

	void RemainingLoads(){
		load -= 2;
		if (load == 0){
			StartCoroutine(this.Reload ());
		}
	}

	IEnumerator Reload(){
		reload.PlayOneShot(reload.clip);
		isReloading = true;
		yield return new WaitForSeconds(reloadTime);
		load = maxload;
		isReloading = false;
	}
}
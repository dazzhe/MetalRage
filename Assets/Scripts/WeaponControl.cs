using UnityEngine;
using System.Collections;

public class WeaponControl : MonoBehaviour {
	private AudioSource reload;
	public GameObject weapon1;
	public GameObject weapon2;
	UnitMotor motor;
	GameObject aimedObject;
	GameObject hinderingObject;
	GameObject targetObject;

	[System.NonSerialized]
	public bool isReloading;

	[System.NonSerialized]
	public bool canShot;

	[System.NonSerialized]
	public float minimumY = -60F;

	[System.NonSerialized]
	public float maximumY = 60F;

	[System.NonSerialized]
	public float rotationY = 0F;

	[System.NonSerialized]
	public float normalrotationY = 0F;

	private float interval = 0.06F;
	private bool cooldown = false;

	[System.NonSerialized]
	public float recoil = 0.4f;
	public float recoilrotationx = 0F;
	public float recoilrotationy = 0F;
	public float mindispersion = 10f;
	public float dispersion;
	public float desiredDispersion;
	public float dispersiongrow = 4f;
	float DispersionCorrection(){
		switch(motor._characterState){
		case UnitMotor.CharacterState.Idle:
			return 1f;
		case UnitMotor.CharacterState.Walking:
			return 1f;
		case UnitMotor.CharacterState.Boosting:
			return 1.3f;
		case UnitMotor.CharacterState.Squatting:
			return 0.5f;
		case UnitMotor.CharacterState.Jumping:
			return 1.5f;
		}
		return 1f;
	}

	private float fixSpeed = 25f;
	private Vector3 center = Vector3.zero;
	Ray ray;
	RaycastHit hit;
	RaycastHit hinderinghit;
	public Vector3 target;
	public float maxrange = 0;

	public int maxload = 80;
	public int load;
	private LayerMask mask;
	private Vector3 originPos;
	public Vector3 shootDirection;

	void Start () {
		motor = GetComponent<UnitMotor>();
		AudioSource[] audioSources = GetComponents<AudioSource>();
		reload = audioSources[2];
		center = new Vector3(Screen.width/2, Screen.height/2, 0);
		load = maxload;
		canShot = true;
		dispersion = mindispersion;
		NormalDisplay.SetReticle(dispersion);
		mask = 1 << 8;
		mask = ~mask;
	}
	
	// Update is called once per frame
	void Update () {
		Elevation ();
		StartCoroutine(ShotControl ());
		RecoilControl();
		DispersionControl();
		if (Input.GetButtonDown("Reload") && load != maxload && !isReloading)
		    StartCoroutine(this.Reload());
	}

	void Elevation(){
		normalrotationY += Input.GetAxis ("Mouse Y") * Configuration.sensitivity * motor.sensimag;
		normalrotationY = Mathf.Clamp (normalrotationY, minimumY, maximumY);
		rotationY = normalrotationY + recoilrotationy;
	}

	IEnumerator ShotControl(){
		if (Input.GetButton("Fire1") && load > 0 && !cooldown && canShot && !isReloading){
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

			originPos = transform.position;
			shootDirection = (target - originPos).normalized;
			if (Physics.Raycast (originPos, shootDirection, out hinderinghit, maxrange, mask)){
			    targetObject = hinderinghit.collider.gameObject;
				target = hinderinghit.point;
			}

			Hit dam = targetObject.GetComponentInParent<Hit>();
			if (dam != null){
				dam.TakeDamage(13, this.gameObject);
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
		if (dispersion < 30f){
			dispersion += dispersiongrow;
		}
		else{
			dispersion = 30f;
		}
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
		yield return new WaitForSeconds(1.5f);
		load = maxload;
		isReloading = false;
	}
}
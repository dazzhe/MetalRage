using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	private PhotonView myPV;
	public GameObject van;
	public GameObject bullet;
	public MeshRenderer _renderer1;
	public MeshRenderer _renderer2;

	WeaponControl weaponctrl;
	UnitMotor motor;

	private bool isZooming;

	// Use this for initialization
	void Start () {
		myPV = PhotonView.Get(this);
		_renderer1.enabled = false;
		_renderer2.enabled = false;
		weaponctrl = van.GetComponent<WeaponControl>();
		motor = van.GetComponent<UnitMotor>();
		isZooming =false;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (myPV.isMine && this.gameObject.tag == "Weapon_R"){
			if (Input.GetButtonDown ("Fire2"))
				HalfZoom();
		}
	}

	void HalfZoom(){
		if (isZooming){
			Camera.main.fieldOfView = 90;
			isZooming = false;
			motor.sensimag = 1f;
		}
		else {
			Camera.main.fieldOfView = 20;
			isZooming = true;
			motor.sensimag = 0.25f;
		}
	}

	void OnDestroy(){
		if (Camera.main != null)
			Camera.main.fieldOfView = 90;
	}

	[RPC]
	void MakeShot(){
		audio.Play();
		StartCoroutine(this.ShowMuzzleFlash());
		StartCoroutine (this.CreateBullet());
	}


	IEnumerator ShowMuzzleFlash(){
		_renderer1.enabled = true;
		_renderer2.enabled = true;
		yield return new WaitForSeconds(Random.Range (0.01f,0.03f));
		_renderer1.enabled = false;
		_renderer2.enabled = false;
	}

	IEnumerator CreateBullet(){
		GameObject bul = GameObject.Instantiate(bullet,transform.position,transform.rotation) as GameObject;
		Bullet _bul = bul.GetComponent<Bullet>();
		_bul.targetPos = weaponctrl.target;
		_bul.originPos = transform.position;
		yield return null;
	}
}
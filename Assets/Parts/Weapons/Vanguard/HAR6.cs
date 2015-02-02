using UnityEngine;
using System.Collections;

public class HAR6 : WeaponRay {
	private bool isZooming;
	
	void Awake () {
		isZooming = false;
		maxload = 80;
		damage = 13;
		recoil = 0.4f;//反動.
		mindispersion = 10f;//ばらつき.
		dispersiongrow = 0.4f;
		maxrange = 1000;
		reloadTime = 1.5f;
		interval = 0.07F;
		Init ();
	}
	
	void LateUpdate () {
		StartCoroutine(ShotControl ());
		if (wcontrol.inputReload && load != maxload && !isReloading)
			StartCoroutine(this.Reload());
		if (wcontrol.inputShot2)
			HalfZoom();
		NormalDisplay.SetReticle(mindispersion * wcontrol.desiredDispersion);
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
}
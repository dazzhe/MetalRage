using UnityEngine;
using System.Collections;

public class IFP40 : WeaponRay {
	private bool isZooming;
	
	void Awake () {
		isZooming = false;
		NormalDisplay.DeleteReticle();
		maxload = 6;
		damage = 200;
		recoil = 10f;//反動.
		mindispersion = 0f;//ばらつき.
		dispersiongrow = 0f;
		maxrange = 1000;
		reloadTime = 3f;
		interval = 2F;
		Init ();
	}

	void LateUpdate () {
		StartCoroutine(ShotControl ());
		if (wcontrol.inputReload && load != maxload && !isReloading)
			StartCoroutine(this.Reload());
		if (wcontrol.inputShot2){
			if (isZooming)
				ZoomOff();
			else
				ZoomOn ();
		}
		NormalDisplay.SetReticle(mindispersion * wcontrol.desiredDispersion);
	}

	void ZoomOn(){
		Camera.main.fieldOfView = 15;
		isZooming = true;
		motor.sensimag = 0.2f;
		NormalDisplay.EnableReticle();
	}
	void ZoomOff(){
		Camera.main.fieldOfView = 90;
		isZooming = false;
		motor.sensimag = 1f;
		NormalDisplay.DeleteReticle();
	}
	void OnDestroy(){
		if (Camera.main != null)
			Camera.main.fieldOfView = 90;
		NormalDisplay.EnableReticle();
	}
}
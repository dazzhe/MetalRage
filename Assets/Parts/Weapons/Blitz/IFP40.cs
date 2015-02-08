using UnityEngine;
using System.Collections;
public class IFP40 : Weapon {
	private bool isZooming = false;
	WeaponRay wr;
	
	void Awake () {
		param.magazine = 6;
		param.damage = 200;
		param.recoil = 6f;
		param.mindispersion = 0f;
		param.dispersiongrow = 0f;
		param.maxrange = 1000;
		param.reloadTime = 3f;
		param.interval = 2F;
		wr = this.gameObject.AddComponent<WeaponRay>();
		wr.param = this.param;
		wr.component = this.component;
		Init ();
		if (component.myPV.isMine)
			NormalDisplay.HideReticle();
		component.wcontrol.isBlitzMain = true;
	}

	protected IEnumerator ShotControl(){
		if (component.wcontrol.inputShot1 && param.load > 0 && !param.cooldown && param.canShot && !param.isReloading){
			wr.RayShot();
			RecoilAndDisperse ();
			RemainingLoads(2);
			StartCoroutine("ZoomOffCoroutine");
			component.myPV.RPC("MakeShots",PhotonTargets.All);
			param.cooldown = true;
			yield return new WaitForSeconds(param.interval);
			param.cooldown = false;
		}
	}

	void LateUpdate () {
		if (component.myPV.isMine){
			StartCoroutine(ShotControl ());
			if (component.wcontrol.inputReload && param.load != param.magazine && !param.isReloading)
				StartCoroutine(this.Reload());
			if (component.wcontrol.inputShot2){
				if (isZooming)
					ZoomOff ();
				else {
					StopCoroutine("ZoomOffCoroutine");
					ZoomOn ();
				}
			}
			NormalDisplay.SetReticle(param.mindispersion * component.wcontrol.desiredDispersion);
		}
	}

	void ZoomOn(){
		Camera.main.fieldOfView = 15;
		isZooming = true;
		component.motor.sensimag = 0.2f;
		param.mindispersion = 0f;
		NormalDisplay.ShowReticle();
	}

	IEnumerator ZoomOffCoroutine(){
		yield return new WaitForSeconds(0.3f);
		ZoomOff ();
	}

	void ZoomOff(){
		Camera.main.fieldOfView = 90;
		isZooming = false;
		component.motor.sensimag = 1f;
		param.mindispersion = 100f;
		NormalDisplay.HideReticle();
	}

	void OnDestroy(){
		if (component.myPV.isMine){
			if (Camera.main != null)
				Camera.main.fieldOfView = 90;
			NormalDisplay.ShowReticle();
		}
	}
	[RPC]
	protected void MakeShots(){
		transform.BroadcastMessage("MakeShot",wr.targetPos);
	}
}
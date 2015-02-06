using UnityEngine;
using System.Collections;

public class HAR6 : Weapon {
	private static bool isZooming = false;
	WeaponRay wr;
	void Awake () {
		param.magazine = 80;
		param.damage = 13;
		param.recoil = 0.4f;//反動.
		param.mindispersion = 10f;//ばらつき.
		param.dispersiongrow = 0.4f;
		param.maxrange = 1000;
		param.reloadTime = 1.5f;
		param.interval = 0.07F;
		wr = this.gameObject.AddComponent<WeaponRay>();
		wr.param = this.param;
		wr.component = this.component;
		Init ();
	}
	
	void LateUpdate () {
		StartCoroutine(ShotControl ());
		if (component.wcontrol.inputReload && param.load != param.magazine && !param.isReloading){
			ZoomOff ();
			StartCoroutine(this.Reload());
		}
		if (component.wcontrol.inputShot2){
			if (isZooming)
				ZoomOff();
			else
				ZoomOn();
		}
		NormalDisplay.SetReticle(param.mindispersion * component.wcontrol.desiredDispersion);
	}
	protected IEnumerator ShotControl(){
		if (component.wcontrol.inputShot1 && param.load > 0 && !param.cooldown && param.canShot && !param.isReloading){
			wr.RayShot();
			RecoilAndDisperse ();
			RemainingLoads(2);
			component.myPV.RPC("MakeShots",PhotonTargets.All,wr.targetPos);
			param.cooldown = true;
			yield return new WaitForSeconds(param.interval);
			param.cooldown = false;
		}
	}
	protected override IEnumerator Reload(){
		component.reload.PlayOneShot(component.reload.clip);
		param.isReloading = true;
		ZoomOff();
		yield return new WaitForSeconds(param.reloadTime);
		param.load = param.magazine;
		param.isReloading = false;
		NormalDisplay.NOLtext.text = param.load.ToString();
	}

	private void ZoomOn(){
		Camera.main.fieldOfView = 20;
		isZooming = true;
		component.motor.sensimag = 0.25f;
	}

	[RPC]
	protected void MakeShots(Vector3 targetPos){
		transform.BroadcastMessage("MakeShot",targetPos);
	}

	private void ZoomOff(){
		Camera.main.fieldOfView = 90;
		isZooming = false;
		component.motor.sensimag = 1f;
	}
	void OnDestroy(){
		if (Camera.main != null && component.myPV.isMine)
			Camera.main.fieldOfView = 90;
	}
}
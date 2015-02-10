using UnityEngine;
using System.Collections;
public class IFP40 : Weapon {
	WeaponRay ray;
	WeaponZoom zoom;


	void Awake () {
		param.ammo = 76;
		param.magazine = 6;
		param.damage = 200;
		param.recoil = 6f;
		param.mindispersion = 0f;
		param.dispersiongrow = 0f;
		param.maxrange = 1000;
		param.reloadTime = 3f;
		param.interval = 2F;
		sightPrefabName = "HAR-6_Sight";
		ray = this.gameObject.AddComponent<WeaponRay>();
		ray.param = this.param;
		ray.component = this.component;
		zoom = this.gameObject.AddComponent<WeaponZoom>();
		zoom.component = this.component;
		zoom.zoomRatio = 10f;
		Init ();
		if (component.myPV.isMine)
			sight.HideSight();
		component.wcontrol.isBlitzMain = true;
	}

	protected IEnumerator ShotControl(){
		if (component.wcontrol.inputShot1 && param.load > 0 && !param.cooldown && param.canShot && !param.isReloading){
			ray.RayShot();
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
				if (zoom.isZooming)
					this.ZoomOff ();
				else
					this.ZoomOn();
			}
			sight.SetArea(param.mindispersion * component.wcontrol.desiredDispersion);
		}
	}

	IEnumerator ZoomOffCoroutine(){
		yield return new WaitForSeconds(0.3f);
		this.ZoomOff ();
	}

	void ZoomOff(){
		zoom.ZoomOff();
		sight.HideSight();
	}

	void ZoomOn(){
		StopCoroutine("ZoomOffCoroutine");
		zoom.ZoomOn();
		sight.ShowSight();
	}

	void OnDestroy(){
		if (component.myPV.isMine)
			sight.ShowSight();
	}

	protected override IEnumerator Reload (){
		this.ZoomOff();
		return base.Reload ();
	}

	protected override void Disable (){
		component.wcontrol.isBlitzMain = false;
		zoom.ZoomOff();
		base.Disable ();
	}

	protected override void Enable (){
		component.wcontrol.isBlitzMain = true;
		base.Enable ();
		sight.HideSight();
	}

	[RPC]
	private void MakeShots(){
		transform.BroadcastMessage("MakeShot",ray.targetPos);
	}
}
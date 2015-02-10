using UnityEngine;
using System.Collections;

public class HAR6 : Weapon {
	WeaponRay ray;
	WeaponZoom zoom;

	void Awake () {
		param.ammo = 1120;
		param.magazine = 80;
		param.damage = 13;
		param.recoil = 0.4f;
		param.mindispersion = 10f;
		param.dispersiongrow = 0.4f;
		param.maxrange = 1000;
		param.reloadTime = 1.5f;
		param.interval = 0.07f;
		sightPrefabName = "HAR-6_Sight";
		ray = this.gameObject.AddComponent<WeaponRay>();
		ray.param = this.param;
		ray.component = this.component;
		zoom = this.gameObject.AddComponent<WeaponZoom>();
		zoom.component = this.component;
		zoom.zoomRatio = 4f;
		Init ();
	}
	
	void LateUpdate () {
		StartCoroutine(ShotControl ());
		if (component.wcontrol.inputReload && param.load != param.magazine && !param.isReloading){
			zoom.ZoomOff ();
			StartCoroutine(this.Reload());
		}
		if (component.wcontrol.inputShot2){
			if (zoom.isZooming)
				zoom.ZoomOff();
			else
				zoom.ZoomOn();
		}
		sight.SetArea(param.mindispersion * component.wcontrol.desiredDispersion * 2);
	}

	protected IEnumerator ShotControl(){
		if (component.wcontrol.inputShot1 && param.load > 0 && !param.cooldown && param.canShot && !param.isReloading){
			ray.RayShot();
			RecoilAndDisperse ();
			RemainingLoads(2);
			component.myPV.RPC("MakeShots",PhotonTargets.All,ray.targetPos);
			param.cooldown = true;
			yield return new WaitForSeconds(param.interval);
			param.cooldown = false;
		}
	}

	protected override IEnumerator Reload(){
		zoom.ZoomOff();
		return base.Reload();
	}

	[RPC]
	private void MakeShots(Vector3 targetPos){
		transform.BroadcastMessage("MakeShot",targetPos);
	}
	
	protected override void Disable (){
		zoom.ZoomOff();
		base.Disable ();
	}
}
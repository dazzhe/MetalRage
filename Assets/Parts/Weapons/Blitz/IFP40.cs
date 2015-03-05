using UnityEngine;
using System.Collections;
public class IFP40 : Weapon {
	WeaponRay ray;
	WeaponZoom zoom;
	GameObject zoomCamera;

	void Awake () {
		param.ammo = 76;
		param.magazine = 6;
		param.damage = 200;
		param.recoilY = 0.5f;
		param.minDispersion = 0f;
		param.dispersionGrow = 0f;
		param.maxrange = 1000;
		param.reloadTime = 3f;
		param.interval = 2F;
		sightPrefabName = "IFP-40_Sight";
		ray = this.gameObject.AddComponent<WeaponRay>();
		ray.param = this.param;
		ray.component = this.component;
		zoom = this.gameObject.AddComponent<WeaponZoom>();
		zoom.component = this.component;
		zoom.zoomRatio = 1.5f;
		Init ();
		if (component.myPV.isMine){
			sight.HideSight();
			zoomCamera = GameObject.Instantiate(Resources.Load("ZoomCamera"),Vector3.zero,Quaternion.identity) as GameObject;
			zoomCamera.transform.parent = Camera.main.transform;
			zoomCamera.transform.localPosition = Vector3.zero;
			zoomCamera.transform.localRotation = Quaternion.identity;
			zoomCamera.SetActive(false);
		}
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
		}
	}

	IEnumerator ZoomOffCoroutine(){
		yield return new WaitForSeconds(0.3f);
		this.ZoomOff ();
	}

	void ZoomOff(){
		zoom.ZoomOff();
		zoomCamera.SetActive(false);
		sight.HideSight();
	}

	void ZoomOn(){
		StopCoroutine("ZoomOffCoroutine");
		zoom.ZoomOn();
		component.motor.sensimag = 0.2f;
		zoomCamera.SetActive(true);
		sight.ShowSight();
	}

	protected override void OnDestroy(){
		base.OnDestroy();
		if (component.myPV.isMine)
			Destroy (zoomCamera);
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
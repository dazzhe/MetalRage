using UnityEngine;
using System.Collections;

public class MAF : Weapon {
	public bool isOpen = true;
	WeaponRay wr;

	// Use this for initialization
	void Awake () {
		param.ammo = 900;
		param.magazine = 900;
		param.damage = 13;
		param.recoil = 0f;
		param.mindispersion = 0f;
		param.dispersiongrow = 0f;
		param.maxrange = 25;
		param.reloadTime = 1.5f;
		param.interval = 0.06F;
		wr = this.gameObject.AddComponent<WeaponRay>();
		wr.param = this.param;
		wr.component = this.component;
		Init ();
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(ShotControl ());
		if (component.wcontrol.inputShot2)
			component.myPV.RPC("Shield",PhotonTargets.AllBuffered);
		NormalDisplay.SetReticle(param.mindispersion * component.wcontrol.desiredDispersion);
	}
	protected IEnumerator ShotControl(){
		if (component.wcontrol.inputShot1 && param.load > 0 && !param.cooldown && param.canShot && !param.isReloading){
			wr.RayShot();
			RecoilAndDisperse ();
			RemainingLoads(2);
			component.myPV.RPC("MakeShots",PhotonTargets.All);
			param.cooldown = true;
			yield return new WaitForSeconds(param.interval);
			param.cooldown = false;
		}
	}

	protected override void Disable (){
		if (!isOpen)
			component.myPV.RPC("Shield",PhotonTargets.AllBuffered);
		base.Disable ();
	}
	[RPC]
	void Shield(){
		if (isOpen){
			isOpen = false;
			param.canShot = false;
			transform.BroadcastMessage("ShieldClose");
		}
		else {
			isOpen = true;
			param.canShot = true;
			transform.BroadcastMessage("ShieldOpen");
		}
	}
	[RPC]
	protected void MakeShots(){
		transform.BroadcastMessage("MakeShot",wr.targetPos);
	}
}
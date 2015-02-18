using UnityEngine;
using System.Collections;

public class MAF : Weapon {
	public bool isOpen = true;
	WeaponRay wr;
	Animator animator;

	void Awake () {
		param.magazine = 900;
		param.damage = 13;
		param.recoil = 0f;
		param.minDispersion = 0f;
		param.dispersionGrow = 0f;
		param.maxrange = 25;
		param.reloadTime = 1.5f;
		param.interval = 0.06F;
		sightPrefabName = "MAF_Sight";
		wr = this.gameObject.AddComponent<WeaponRay>();
		wr.param = this.param;
		wr.component = this.component;
		Init ();
		if (component.myPV.isMine)
			animator = sight.GetComponent<Animator>();
	}

	void Update () {
		StartCoroutine(ShotControl ());
		if (component.wcontrol.inputShot2)
			component.myPV.RPC("Shield",PhotonTargets.AllBuffered);
	}

	protected IEnumerator ShotControl(){
		if (component.wcontrol.inputShot1 && param.load > 0 && !param.cooldown && param.canShot && !param.isReloading){
			animator.SetBool("rotate",true);
			wr.RayShot();
			RecoilAndDisperse ();
			RemainingLoads(2);
			component.myPV.RPC("MakeShots",PhotonTargets.All);
			param.cooldown = true;
			yield return new WaitForSeconds(param.interval);
			param.cooldown = false;
		} else if (component.wcontrol.inputShot1 != true)
			animator.SetBool("rotate",false);
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
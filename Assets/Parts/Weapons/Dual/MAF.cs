using UnityEngine;
using System.Collections;

public class MAF : WeaponRay {
	public bool isOpen = true;

	// Use this for initialization
	void Awake () {
		maxload = 800;
		damage = 13;
		recoil = 0f;//反動.
		mindispersion = 0f;//ばらつき.
		dispersiongrow = 0f;
		maxrange = 10;
		reloadTime = 1.5f;
		interval = 0.06F;
		Init ();
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(ShotControl ());
        if (wcontrol.inputShot2)
			myPV.RPC("Shield",PhotonTargets.AllBuffered);
		NormalDisplay.SetReticle(mindispersion * wcontrol.desiredDispersion);
	}
	
	[RPC]
	void Shield(){
		if (isOpen){
			isOpen = false;
			canShot = false;
			transform.BroadcastMessage("ShieldClose");
		}
		else {
			isOpen = true;
			canShot = true;
			transform.BroadcastMessage("ShieldOpen");
		}
	}
}
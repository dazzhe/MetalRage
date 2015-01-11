using UnityEngine;
using System.Collections;

public class DualInitialization : MonoBehaviour {
	private PhotonView myPV;
	WeaponControl wc;
	Status stat;

	void Awake () {
		wc = GetComponent<WeaponControl>();
		stat = GetComponent<Status>();
		myPV = PhotonView.Get (this);
		if (myPV.isMine){
			GetComponent<UnitController>().enabled = true;
			GetComponent<UnitMotor>().enabled = true;
			wc.enabled = true;
			GetComponent<MainCamera>().enabled = true;
		}
		wc.maxload = 900;
		wc.maxrange = 27;
		wc.recoil = 0;
		wc.dispersiongrow = 0;
		wc.mindispersion = 0;
		stat.maxHP = 268;
	}
}

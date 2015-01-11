using UnityEngine;
using System.Collections;

public class VanguardInitialization : MonoBehaviour {
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

		wc.maxrange = 1000;
		stat.maxHP = 280;
	}
}

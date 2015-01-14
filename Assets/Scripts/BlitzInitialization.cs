using UnityEngine;
using System.Collections;

public class BlitzInitialization : MonoBehaviour {
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
		wc.maxload = 6;
		wc.maxrange = 2000;
		wc.damage = 200;
		wc.interval = 1.5f;
		wc.reloadTime = 3f;
		wc.recoil = 0f;
		stat.maxHP = 255;
	}
}

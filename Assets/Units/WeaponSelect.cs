using UnityEngine;
using System.Collections;

public class WeaponSelect : MonoBehaviour {
	private GameObject weapon;
	void Awake () {
		weapon = transform.GetChild(0).gameObject;
	}
	
	public void SendEnable (){
		weapon.SendMessage("Enable");
	}
	
	public void SendDisable(){
		weapon.SendMessage("Disable");
	}
}

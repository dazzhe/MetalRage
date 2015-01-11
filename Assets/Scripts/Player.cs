using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public GameObject van;
	
	WeaponControl weaponctrl;



	// Use this for initialization
	void Start () {
		weaponctrl = van.GetComponent<WeaponControl>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.localEulerAngles = new Vector3 (-weaponctrl.rotationY, weaponctrl.recoilrotationx, 0);
	}
}

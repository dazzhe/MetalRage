using UnityEngine;
using System.Collections;
//武器を垂直に回転させる.
public class Elevation : MonoBehaviour {
	public GameObject unit;
	WeaponControl weaponctrl;

	void Start () {
		weaponctrl = unit.GetComponent<WeaponControl>();
	}
	
	void Update () {
		transform.localEulerAngles = new Vector3 (-weaponctrl.rotationY, weaponctrl.recoilrotationx, 0);
	}
}

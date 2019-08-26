using UnityEngine;

public class Elevation : MonoBehaviour {
    public GameObject unit;
    private WeaponControl weaponControl;

    private void Start() {
        this.weaponControl = this.unit.GetComponent<WeaponControl>();
    }

    private void Update() {
        this.transform.localEulerAngles = new Vector3(-this.weaponControl.rotationY, this.weaponControl.recoilrotationx, 0);
    }
}

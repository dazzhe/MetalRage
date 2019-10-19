using UnityEngine;

public class Elevation : MonoBehaviour {
    public GameObject unit;
    private Robot weaponControl;

    private void Start() {
        this.weaponControl = this.unit.GetComponent<Robot>();
    }

    private void Update() {
        this.transform.localEulerAngles = new Vector3(-this.weaponControl.RotationY, this.weaponControl.RecoilRotation.x, 0);
    }
}

using UnityEngine;

public class Elevation : MonoBehaviour {
    public GameObject unit;
    private Robot robot;

    private void Start() {
        this.robot = this.unit.GetComponent<Robot>();
    }

    private void Update() {
        this.transform.localEulerAngles = new Vector3(-this.robot.RotationY, this.robot.RecoilRotation.x, 0);
    }
}

using UnityEngine;

public class Elevation : MonoBehaviour {
    [SerializeField]
    private GameObject root = default;
    private Robot robot;

    private void Awake() {
        this.robot = this.root.GetComponent<Robot>();
    }

    private void Update() {
        this.transform.localEulerAngles = new Vector3(-this.robot.RotationY, this.robot.RecoilRotation.x, 0);
    }
}

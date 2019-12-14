using UnityEngine;

public class Elevation : MonoBehaviour {
    [SerializeField]
    private GameObject root = default;

    private Mech robot;

    private void Awake() {
        this.robot = this.root.GetComponent<Mech>();
    }

    private void Update() {
        this.transform.localEulerAngles = new Vector3(-this.robot.RotationY, this.robot.RecoilRotation.x, 0);
    }
}

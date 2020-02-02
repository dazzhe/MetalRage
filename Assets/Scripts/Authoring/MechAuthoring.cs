using UnityEngine;

public class MechAuthoring : MonoBehaviour {
    [SerializeField]
    private Transform cameraTarge
        t;

    public Transform CameraTarget { get => this.cameraTarget; set => this.cameraTarget = value; }
}

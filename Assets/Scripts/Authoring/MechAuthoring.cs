using UnityEngine;

public class MechAuthoring : MonoBehaviour {
    [SerializeField]
    private Transform cameraTarget;

    public Transform CameraTarget { get => this.cameraTarget; set => this.cameraTarget = value; }
}

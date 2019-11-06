using UnityEngine;

public class UpperBody : MonoBehaviour {
    [SerializeField]
    private GameObject lowerBody = default;

    private Vector3 offset;

    private void Start() {
        this.offset = this.transform.localPosition - this.transform.parent.InverseTransformPoint(this.lowerBody.transform.position);
    }

    private void Update() {
        this.transform.localPosition = this.transform.parent.InverseTransformPoint(this.lowerBody.transform.position) + this.offset;
    }
}

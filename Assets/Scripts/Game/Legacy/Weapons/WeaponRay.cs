using UnityEngine;

public class WeaponRay {
    private Camera camera;
    private Transform origin;
    private float maxDistance;

    public WeaponRay(Camera camera, Transform origin, float maxDistance) {
        this.camera = camera;
        this.origin = origin;
        this.maxDistance = maxDistance;
    }

    public RaycastHit Raycast(Vector2 screenPosition) {
        Ray ray = this.camera.ScreenPointToRay(screenPosition);
        var endPosition = ray.GetPoint(this.maxDistance);
        var rayDirection = (endPosition - this.origin.position).normalized;
        Physics.Raycast(this.origin.position, rayDirection, out RaycastHit hit, this.maxDistance);
        return hit;
    }
}

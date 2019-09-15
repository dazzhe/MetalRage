using UnityEngine;

public class WeaponZoom : MonoBehaviour {
    public bool isZoomed = false;
    public WeaponComponent component = new WeaponComponent();
    public float zoomRatio;

    public void ZoomIn() {
        Camera.main.fieldOfView = 90f / this.zoomRatio;
        this.isZoomed = true;
        this.component.motor.sensimag = 1f / this.zoomRatio;
    }

    public void ZoomOut() {
        Camera.main.fieldOfView = 90f;
        this.isZoomed = false;
        this.component.motor.sensimag = 1f;
    }

    private void OnDestroy() {
        if (Camera.main != null && this.component.myPV.isMine) {
            Camera.main.fieldOfView = 90f;
        }
    }
}
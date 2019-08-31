using UnityEngine;

public class WeaponZoom : MonoBehaviour {
    public bool isZooming = false;
    public WeaponComponent component = new WeaponComponent();
    public float zoomRatio;

    public void ZoomOn() {
        Camera.main.fieldOfView = 90f / this.zoomRatio;
        this.isZooming = true;
        this.component.motor.sensimag = 1f / this.zoomRatio;
    }

    public void ZoomOff() {
        Camera.main.fieldOfView = 90;
        this.isZooming = false;
        this.component.motor.sensimag = 1f;
    }

    private void OnDestroy() {
        if (Camera.main != null && this.component.myPV.isMine) {
            Camera.main.fieldOfView = 90;
        }
    }
}
using UnityEngine;

public class WeaponZoom {
    public float sensitivityScale = 0.2f;
    public bool isZoomed = false;
    public float zoomRatio;

    public void ZoomIn() {
        Camera.main.fieldOfView = 90f / this.zoomRatio;
        this.isZoomed = true;
    }

    public void Toggle() {
        if (this.isZoomed) {
            ZoomOut();
        } else {
            ZoomIn();
        }
    }

    public void ZoomOut() {
        Camera.main.fieldOfView = 90f;
        this.isZoomed = false;
    }
}
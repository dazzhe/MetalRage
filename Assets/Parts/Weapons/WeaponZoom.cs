using UnityEngine;
using System.Collections;

public class WeaponZoom : MonoBehaviour {
	public bool isZooming = false;
	public WeaponComponent component = new WeaponComponent();
	public float zoomRatio;

	public void ZoomOn(){
		Camera.main.fieldOfView = 90f / zoomRatio;
		isZooming = true;
		component.motor.sensimag = 1f / zoomRatio;
	}

	public void ZoomOff(){
		Camera.main.fieldOfView = 90;
		isZooming = false;
		component.motor.sensimag = 1f;
	}

	void OnDestroy(){
		if (Camera.main != null && component.myPV.isMine)
			Camera.main.fieldOfView = 90;
	}
}
using System.Collections;
using UnityEngine;
public class IFP40 : Weapon {
    private WeaponRay ray;
    private WeaponZoom zoom;
    private GameObject zoomCamera;

    private void Awake() {
        this.param.ammo = 76;
        this.param.magazine = 6;
        this.param.damage = 200;
        this.param.recoilY = 0.5f;
        this.param.minDispersion = 0f;
        this.param.dispersionGrow = 0f;
        this.param.maxrange = 1000;
        this.param.reloadTime = 3f;
        this.param.interval = 2F;
        this.sightPrefabName = "IFP-40_Sight";
        this.ray = this.gameObject.AddComponent<WeaponRay>();
        this.ray.param = this.param;
        this.ray.component = this.component;
        this.zoom = this.gameObject.AddComponent<WeaponZoom>();
        this.zoom.component = this.component;
        this.zoom.zoomRatio = 1.5f;
        Init();
        if (this.component.myPV.isMine) {
            this.sight.HideSight();
            this.zoomCamera = GameObject.Instantiate(Resources.Load("ZoomCamera"), Vector3.zero, Quaternion.identity) as GameObject;
            this.zoomCamera.transform.parent = Camera.main.transform;
            this.zoomCamera.transform.localPosition = Vector3.zero;
            this.zoomCamera.transform.localRotation = Quaternion.identity;
            this.zoomCamera.SetActive(false);
        }
        this.component.wcontrol.isBlitzMain = true;
    }

    protected IEnumerator ShotControl() {
        if (this.component.wcontrol.inputShot1 && this.param.load > 0 && !this.param.cooldown && this.param.canShot && !this.param.isReloading) {
            this.ray.RayShot();
            RecoilAndDisperse();
            RemainingLoads(2);
            StartCoroutine("ZoomOffCoroutine");
            this.component.myPV.RPC("MakeShots", PhotonTargets.All);
            this.param.cooldown = true;
            yield return new WaitForSeconds(this.param.interval);
            this.param.cooldown = false;
        }
    }

    private void LateUpdate() {
        if (this.component.myPV.isMine) {
            StartCoroutine(ShotControl());
            if (this.component.wcontrol.inputReload && this.param.load != this.param.magazine && !this.param.isReloading) {
                StartCoroutine(Reload());
            }

            if (this.component.wcontrol.inputShot2) {
                if (this.zoom.isZooming) {
                    ZoomOff();
                } else {
                    ZoomOn();
                }
            }
        }
    }

    private IEnumerator ZoomOffCoroutine() {
        yield return new WaitForSeconds(0.3f);
        ZoomOff();
    }

    private void ZoomOff() {
        this.zoom.ZoomOff();
        this.zoomCamera.SetActive(false);
        this.sight.HideSight();
    }

    private void ZoomOn() {
        StopCoroutine("ZoomOffCoroutine");
        this.zoom.ZoomOn();
        this.component.motor.sensimag = 0.2f;
        this.zoomCamera.SetActive(true);
        this.sight.Show();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (this.component.myPV.isMine) {
            Destroy(this.zoomCamera);
        }
    }

    protected override IEnumerator Reload() {
        ZoomOff();
        return base.Reload();
    }

    protected override void Disable() {
        this.component.wcontrol.isBlitzMain = false;
        this.zoom.ZoomOff();
        base.Disable();
    }

    protected override void Enable() {
        this.component.wcontrol.isBlitzMain = true;
        base.Enable();
        this.sight.HideSight();
    }

    [PunRPC]
    private void MakeShots() {
        this.transform.BroadcastMessage("MakeShot", this.ray.targetPos);
    }
}
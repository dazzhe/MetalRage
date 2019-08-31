using System.Collections;
using UnityEngine;

public class HAR6 : Weapon {
    private WeaponRay ray;
    private WeaponZoom zoom;

    private void Awake() {
        this.param.ammo = 1120;
        this.param.magazine = 80;
        this.param.damage = 13;
        this.param.recoilY = 0.4f;
        this.param.recoilX = 0.4f;
        this.param.minDispersion = 10f;
        this.param.dispersionGrow = 0.4f;
        this.param.maxrange = 1000;
        this.param.reloadTime = 1.5f;
        this.param.interval = 0.07f;
        this.ray = this.gameObject.AddComponent<WeaponRay>();
        this.ray.param = this.param;
        this.ray.component = this.component;
        this.zoom = this.gameObject.AddComponent<WeaponZoom>();
        this.zoom.component = this.component;
        this.zoom.zoomRatio = 4f;
        Init();
    }

    private void LateUpdate() {
        StartCoroutine(ShotControl());
        if (this.component.wcontrol.inputReload && this.param.load != this.param.magazine && !this.param.isReloading) {
            this.zoom.ZoomOff();
            StartCoroutine(Reload());
        }
        if (this.component.wcontrol.inputShot2) {
            if (this.zoom.isZooming) {
                this.zoom.ZoomOff();
            } else {
                this.zoom.ZoomOn();
            }
        }
        this.sight.extent = this.param.minDispersion * this.component.wcontrol.desiredDispersion * 2;
    }

    protected IEnumerator ShotControl() {
        if (this.component.wcontrol.inputShot1 && this.param.load > 0 && !this.param.cooldown && this.param.canShot && !this.param.isReloading) {
            this.ray.RayShot();
            RecoilAndDisperse();
            RemainingLoads(2);
            this.component.myPV.RPC("MakeShots", PhotonTargets.All, this.ray.targetPos);
            this.param.cooldown = true;
            yield return new WaitForSeconds(this.param.interval);
            this.param.cooldown = false;
        }
    }

    protected override IEnumerator Reload() {
        this.zoom.ZoomOff();
        return base.Reload();
    }

    [PunRPC]
    private void MakeShots(Vector3 targetPos) {
        this.transform.BroadcastMessage("MakeShot", targetPos);
    }

    protected override void Disable() {
        this.zoom.ZoomOff();
        base.Disable();
    }
}
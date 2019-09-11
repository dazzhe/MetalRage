using System.Collections;
using UnityEngine;

public class HAR6 : Weapon {
    private WeaponRay ray;
    private WeaponZoom zoom;

    private void Awake() {
        this.Ammo.ReserveBulletCount = 1200;
        this.Ammo.MagazineSize = 80;
        this.Ammo.Reload();
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
        if (this.component.wcontrol.inputReload && this.Ammo.CanReload && !this.param.isReloading) {
            this.zoom.ZoomOut();
            StartCoroutine(Reload());
        }
        if (this.component.wcontrol.inputShot2) {
            if (this.zoom.isZoomed) {
                this.zoom.ZoomOut();
            } else {
                this.zoom.ZoomIn();
            }
        }
        this.Crosshair.extent = this.param.minDispersion * this.component.wcontrol.Dispersion * 2f;
    }

    protected IEnumerator ShotControl() {
        if (this.component.wcontrol.inputShot1 && !this.Ammo.IsMagazineEmpty && !this.param.cooldown && !this.param.isReloading) {
            this.ray.RayShot();
            RecoilAndDisperse();
            ConsumeBullets(2);
            this.component.myPV.RPC("MakeShots", PhotonTargets.All, this.ray.targetPos);
            this.param.cooldown = true;
            yield return new WaitForSeconds(this.param.interval);
            this.param.cooldown = false;
        }
    }

    protected override IEnumerator Reload() {
        this.zoom.ZoomOut();
        return base.Reload();
    }

    [PunRPC]
    private void MakeShots(Vector3 targetPos) {
        this.transform.BroadcastMessage("MakeShot", targetPos);
    }

    public override void Unselect() {
        this.zoom.ZoomOut();
        base.Unselect();
    }
}
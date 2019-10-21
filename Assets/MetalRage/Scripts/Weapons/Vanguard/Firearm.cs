using System.Collections;
using UnityEngine;

public class Firearm : Weapon {
    private WeaponZoom zoom;

    [SerializeField]
    private Transform shotOrigin;

    private void Awake() {
        this.Ammo.ReserveBulletCount = 1200;
        this.Ammo.MagazineSize = 80;
        this.Ammo.Reload();
        this.param.damage = 13;
        this.param.recoil = new Vector2(0.4f, 0.4f);
        this.Spread.MinAngle = 10f;
        this.Spread.GrowRate = 0.1f;
        this.ray = new WeaponRay(Camera.main, this.shotOrigin, 1000f);
        this.param.reloadTime = 1.5f;
        this.param.coolDownTime = 0.07f;
        this.zoom.zoomRatio = 4f;
        Initialize();
    }

    private void LateUpdate() {
        StartCoroutine(ShotControl());
        if (this.robot.UserCommand.Reload && this.Ammo.CanReload && !this.param.isReloading) {
            this.zoom.ZoomOut();
            StartCoroutine(ReloadRoutine());
        }
        if (this.robot.UserCommand.Fire2) {
            this.zoom.Toggle();
            this.SensitivityScale = this.zoom.isZoomed ? this.zoom.sensitivityScale : 1f;
        }
        this.Crosshair.UpdateConeOfFire(this.Spread);
    }

    protected IEnumerator ShotControl() {
        if (this.robot.UserCommand.Fire1 && !this.Ammo.IsMagazineEmpty && !this.param.isCoolDown && !this.param.isReloading) {
            var hit = this.ray.Raycast(this.Spread.GetSampleInScreen(this.unitMotor.characterState));
            RecoilAndSpread();
            ConsumeBullets(2);
            this.photonView.RPC("MakeShots", PhotonTargets.All, hit.point);
            this.param.isCoolDown = true;
            yield return new WaitForSeconds(this.param.coolDownTime);
            this.param.isCoolDown = false;
        }
    }

    protected override IEnumerator ReloadRoutine() {
        this.zoom.ZoomOut();
        return base.ReloadRoutine();
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
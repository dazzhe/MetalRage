using System.Collections;
using UnityEngine;
public class IFP40 : Weapon {
    [SerializeField]
    private GameObject zoomCameraPrefab = default;
    [SerializeField]
    private Transform shotOrigin = default;

    private GameObject zoomCamera;
    private WeaponZoom zoom = new WeaponZoom();

    private void Awake() {
        this.Ammo.ReserveBulletCount = 82;
        this.Ammo.MagazineSize = 6;
        this.Ammo.Reload();
        this.param.damage = 200;
        this.param.recoil = new Vector2(0f, 0.5f);
        this.Spread.MinAngle = 0f;
        this.param.reloadTime = 3f;
        this.param.coolDownTime = 2f;
        this.ray = new WeaponRay(Camera.main, this.shotOrigin, 1000f);
        this.zoom.zoomRatio = 1.5f;
        Initialize();
        if (this.photonView.isMine) {
            this.Crosshair.Hide();
            this.zoomCamera = Instantiate(this.zoomCameraPrefab, Vector3.zero, Quaternion.identity);
            this.zoomCamera.transform.parent = Camera.main.transform;
            this.zoomCamera.transform.localPosition = Vector3.zero;
            this.zoomCamera.transform.localRotation = Quaternion.identity;
            this.zoomCamera.SetActive(false);
        }
        this.robot.HideEnemyName = true;
    }

    protected IEnumerator ShotControl() {
        var canShot = !this.Ammo.IsMagazineEmpty && !this.param.isCoolDown && !this.param.isReloading;
        if (this.robot.UserCommand.Fire1 && canShot) {
            var hit = this.ray.Raycast(this.Spread.GetSampleInScreen(this.unitMotor.characterState));
            RecoilAndSpread();
            ConsumeBullets(2);
            StartCoroutine(ZoomOutRoutine());
            this.photonView.RPC("MakeShots", PhotonTargets.All, hit.point);
            this.param.isCoolDown = true;
            yield return new WaitForSeconds(this.param.coolDownTime);
            this.param.isCoolDown = false;
        }
    }

    private void LateUpdate() {
        if (this.photonView.isMine) {
            StartCoroutine(ShotControl());
            if (this.robot.UserCommand.Reload && this.Ammo.CanReload && !this.param.isReloading) {
                StartCoroutine(ReloadRoutine());
            }
            if (this.robot.UserCommand.Fire1) {
                if (this.zoom.isZoomed) {
                    ZoomOut();
                } else {
                    ZoomIn();
                }
            }
        }
    }

    private IEnumerator ZoomOutRoutine() {
        yield return new WaitForSeconds(0.3f);
        ZoomOut();
    }

    private void ZoomOut() {
        this.zoom.ZoomOut();
        this.zoomCamera.SetActive(false);
        this.Crosshair.Hide();
    }

    private void ZoomIn() {
        StopCoroutine(ZoomOutRoutine());
        this.zoom.ZoomIn();
        this.zoomCamera.SetActive(true);
        this.Crosshair.Show();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (this.photonView.isMine) {
            Destroy(this.zoomCamera);
        }
    }

    protected override IEnumerator ReloadRoutine() {
        ZoomOut();
        return base.ReloadRoutine();
    }

    public override void Unselect() {
        this.robot.HideEnemyName = false;
        this.zoom.ZoomOut();
        base.Unselect();
    }

    public override void Select() {
        this.robot.HideEnemyName = true;
        base.Select();
        this.Crosshair.Hide();
    }

    [PunRPC]
    private void MakeShots(Vector3 targetPosition) {
        this.transform.BroadcastMessage("MakeShot", targetPosition);
    }
}
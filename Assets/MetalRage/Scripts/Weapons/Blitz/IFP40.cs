using System.Collections;
using UnityEngine;
public class IFP40 : Weapon {
    [SerializeField]
    private GameObject zoomCameraPrefab = default;

    private GameObject zoomCamera;
    private WeaponRay ray;
    private WeaponZoom zoom;

    private void Awake() {
        this.Ammo.ReserveBulletCount = 82;
        this.Ammo.MagazineSize = 6;
        this.Ammo.Reload();
        this.param.damage = 200;
        this.param.recoilY = 0.5f;
        this.param.minDispersion = 0f;
        this.param.dispersionGrow = 0f;
        this.param.maxrange = 1000;
        this.param.reloadTime = 3f;
        this.param.interval = 2F;
        this.ray = this.gameObject.AddComponent<WeaponRay>();
        this.ray.param = this.param;
        this.ray.component = this.component;
        this.zoom = this.gameObject.AddComponent<WeaponZoom>();
        this.zoom.component = this.component;
        this.zoom.zoomRatio = 1.5f;
        Init();
        if (this.component.myPV.isMine) {
            this.Crosshair.Hide();
            this.zoomCamera = Instantiate(this.zoomCameraPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            this.zoomCamera.transform.parent = Camera.main.transform;
            this.zoomCamera.transform.localPosition = Vector3.zero;
            this.zoomCamera.transform.localRotation = Quaternion.identity;
            this.zoomCamera.SetActive(false);
        }
        this.component.wcontrol.isBlitzMain = true;
    }

    protected IEnumerator ShotControl() {
        var canShot = !this.Ammo.IsMagazineEmpty && !this.param.cooldown && !this.param.isReloading;
        if (this.component.wcontrol.inputShot1 && canShot) {
            this.ray.RayShot();
            RecoilAndDisperse();
            ConsumeBullets(2);
            StartCoroutine(ZoomOutRoutine());
            this.component.myPV.RPC("MakeShots", PhotonTargets.All);
            this.param.cooldown = true;
            yield return new WaitForSeconds(this.param.interval);
            this.param.cooldown = false;
        }
    }

    private void LateUpdate() {
        if (this.component.myPV.isMine) {
            StartCoroutine(ShotControl());
            if (this.component.wcontrol.inputReload && this.Ammo.CanReload && !this.param.isReloading) {
                StartCoroutine(Reload());
            }

            if (this.component.wcontrol.inputShot2) {
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
        if (this.component.myPV.isMine) {
            Destroy(this.zoomCamera);
        }
    }

    protected override IEnumerator Reload() {
        ZoomOut();
        return base.Reload();
    }

    public override void Unselect() {
        this.component.wcontrol.isBlitzMain = false;
        this.zoom.ZoomOut();
        base.Unselect();
    }

    public override void Select() {
        this.component.wcontrol.isBlitzMain = true;
        base.Select();
        this.Crosshair.Hide();
    }

    [PunRPC]
    private void MakeShots() {
        this.transform.BroadcastMessage("MakeShot", this.ray.targetPos);
    }
}
using System.Collections;
using UnityEngine;

public class DCDD51 : Weapon {
    [SerializeField]
    private Transform leftMuzzlePoint;
    [SerializeField]
    private Transform rightrMuzzlePoint;
    [SerializeField]
    private AudioClip shotClip;
    //[SerializeField]
    //private AudioClip 

    private void Awake() {
        this.Ammo.ReserveBulletCount = 64;
        this.Ammo.MagazineSize = 8;
        this.Ammo.Reload();
        this.param.damage = 36;
        this.param.recoilY = 2f;
        this.param.recoilX = 2f;
        this.param.minDispersion = 20f;
        this.param.dispersionGrow = 0.4f;
        this.param.maxrange = 1000;
        this.param.reloadTime = 1.5f;
        this.param.interval = 0.5f;
        Init();
    }

    private void LateUpdate() {
        StartCoroutine(ShotControl());
        if (this.component.wcontrol.inputReload && this.Ammo.CanReload && !this.param.isReloading) {
            StartCoroutine(Reload());
        }
        this.Crosshair.extent = this.param.minDispersion * this.component.wcontrol.Dispersion * 2f;
    }

    protected IEnumerator ShotControl() {
        if (this.component.wcontrol.inputShot1 && !this.Ammo.IsMagazineEmpty && !this.param.cooldown && !this.param.isReloading) {
            RecoilAndDisperse();
            ConsumeBullets(2);
            //this.component.myPV.RPC("MakeShots", PhotonTargets.All, this.ray.targetPos);
            this.param.cooldown = true;
            yield return new WaitForSeconds(this.param.interval);
            this.param.cooldown = false;
        }
    }

    [PunRPC]
    private void MakeShots(Vector3 targetPos) {
        this.transform.BroadcastMessage("MakeShot", targetPos);
    }
}
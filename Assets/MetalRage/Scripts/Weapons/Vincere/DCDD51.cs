//using System.Collections;
//using UnityEngine;

//public class DCDD51 : Weapon {
//    [SerializeField]
//    private Transform leftMuzzlePoint;
//    [SerializeField]
//    private Transform rightMuzzlePoint;
//    [SerializeField]
//    private AudioClip shotClip;

//    private void Awake() {
//        this.Ammo.ReserveBulletCount = 64;
//        this.Ammo.MagazineSize = 8;
//        this.Ammo.Reload();
//        this.param.damage = 36;
//        this.param.recoilY = 2f;
//        this.param.recoilX = 2f;
//        this.param.baseSpread = 20f;
//        this.param.dispersionGrow = 0.4f;
//        this.param.maxrange = 1000;
//        this.param.reloadTime = 1.5f;
//        this.param.coolDownTime = 0.5f;
//        Initialize();
//    }

//    private void LateUpdate() {
//        StartCoroutine(ShotControl());
//        if (this.component.wcontrol.inputReload && this.Ammo.CanReload && !this.param.isReloading) {
//            StartCoroutine(ReloadRoutine());
//        }
//        this.Crosshair.CornOfFireSize = this.param.baseSpread * this.component.wcontrol.Dispersion * 2f;
//    }

//    protected IEnumerator ShotControl() {
//        if (this.component.wcontrol.inputShot1 && !this.Ammo.IsMagazineEmpty && !this.param.isCoolDown && !this.param.isReloading) {
//            RecoilAndSpread();
//            ConsumeBullets(2);
//            //this.component.myPV.RPC("MakeShots", PhotonTargets.All, this.ray.targetPos);
//            this.param.isCoolDown = true;
//            yield return new WaitForSeconds(this.param.coolDownTime);
//            this.param.isCoolDown = false;
//        }
//    }

//    [PunRPC]
//    private void MakeShots(Vector3 targetPos) {
//        this.transform.BroadcastMessage("MakeShot", targetPos);
//    }
//}
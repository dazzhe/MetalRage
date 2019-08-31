using System.Collections;
using UnityEngine;

public class MAF : Weapon {
    public bool isOpen = true;
    private WeaponRay wr;
    private Animator animator;

    private void Awake() {
        this.param.magazine = 900;
        this.param.damage = 13;
        this.param.recoilY = 0f;
        this.param.minDispersion = 0f;
        this.param.dispersionGrow = 0f;
        this.param.maxrange = 25;
        this.param.reloadTime = 1.5f;
        this.param.interval = 0.06f;
        this.wr = this.gameObject.AddComponent<WeaponRay>();
        this.wr.param = this.param;
        this.wr.component = this.component;
        Init();
        if (this.component.myPV.isMine) {
            this.animator = this.sight.GetComponent<Animator>();
        }
    }

    private void Update() {
        StartCoroutine(ShotControl());
        if (this.component.wcontrol.inputShot2) {
            this.component.myPV.RPC("Shield", PhotonTargets.AllBuffered);
        }
    }

    protected IEnumerator ShotControl() {
        if (this.component.wcontrol.inputShot1 && this.param.load > 0 && !this.param.cooldown && this.param.canShot && !this.param.isReloading) {
            this.animator.SetBool("rotate", true);
            this.wr.RayShot();
            RecoilAndDisperse();
            RemainingLoads(2);
            this.component.myPV.RPC("MakeShots", PhotonTargets.All);
            this.param.cooldown = true;
            yield return new WaitForSeconds(this.param.interval);
            this.param.cooldown = false;
        } else if (this.component.wcontrol.inputShot1 != true) {
            this.animator.SetBool("rotate", false);
        }
    }

    protected override void Disable() {
        if (!this.isOpen) {
            this.component.myPV.RPC("Shield", PhotonTargets.AllBuffered);
        }

        base.Disable();
    }

    [PunRPC]
    private void Shield() {
        if (this.isOpen) {
            this.isOpen = false;
            this.param.canShot = false;
            this.transform.BroadcastMessage("ShieldClose");
        } else {
            this.isOpen = true;
            this.param.canShot = true;
            this.transform.BroadcastMessage("ShieldOpen");
        }
    }

    [PunRPC]
    protected void MakeShots() {
        this.transform.BroadcastMessage("MakeShot", this.wr.targetPos);
    }
}
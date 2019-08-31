using System.Collections;
using UnityEngine;

public class MAF : Weapon {
    public bool isOpen = true;

    private WeaponRay wr;
    private Animator animator;
    private Animator sightAnimator;
    private new Collider collider;
    private ParticleSystem[] particleSystems;

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
        this.animator = GetComponent<Animator>();
        this.sightAnimator = this.sight.GetComponent<Animator>();
        this.collider = GetComponent<Collider>();
        this.particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void Update() {
        StartCoroutine(ShotControl());
        if (this.component.wcontrol.inputShot2) {
            this.component.myPV.RPC("Shield", PhotonTargets.AllBuffered);
        }
    }

    protected IEnumerator ShotControl() {
        if (this.component.wcontrol.inputShot1 && this.param.load > 0 && !this.param.cooldown && this.param.canShot && !this.param.isReloading) {
            this.sightAnimator.SetBool("rotate", true);
            this.wr.RayShot();
            RecoilAndDisperse();
            RemainingLoads(2);
            this.component.myPV.RPC("MakeShots", PhotonTargets.All);
            this.param.cooldown = true;
            yield return new WaitForSeconds(this.param.interval);
            this.param.cooldown = false;
        } else if (this.component.wcontrol.inputShot1 != true) {
            this.sightAnimator.SetBool("rotate", false);
        }
    }

    protected override void Disable() {
        if (!this.isOpen) {
            this.component.myPV.RPC("Shield", PhotonTargets.AllBuffered);
        }

        base.Disable();
    }


    private void MakeShot() {
        //audio.Play();
        StartCoroutine(EmitFire());
    }

    private void CloseShield() {
        this.animator.SetBool("IsOpen", false);
        this.collider.enabled = true;
        this.collider.isTrigger = true;
    }

    private void OpenShield() {
        this.animator.SetBool("IsOpen", true);
        this.collider.enabled = false;
        this.collider.isTrigger = false;
    }

    private IEnumerator EmitFire() {
        foreach (var particleSystem in this.particleSystems) {
            particleSystem.Play();
        }
        yield return new WaitForSeconds(0.1f);
        foreach (var particleSystem in this.particleSystems) {
            particleSystem.Stop();
        }
    }

    [PunRPC]
    private void Shield() {
        if (this.isOpen) {
            this.isOpen = false;
            this.param.canShot = false;
            CloseShield();
        } else {
            this.isOpen = true;
            this.param.canShot = true;
            OpenShield();
        }
    }

    [PunRPC]
    protected void MakeShots() {
        this.transform.BroadcastMessage("MakeShot", this.wr.targetPos);
    }
}
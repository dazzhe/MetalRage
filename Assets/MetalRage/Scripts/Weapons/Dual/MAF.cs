using System.Collections;
using UnityEngine;

public class MAF : Weapon {
    [SerializeField]
    private AudioClip shieldOpenClip;
    [SerializeField]
    private AudioClip shieldCloseClip;

    private bool isOpen = false;
    private WeaponRay wr;
    private Animator animator;
    private Animator sightAnimator;
    private new AudioSource audio;
    private new Collider collider;
    private ParticleSystem[] particleSystems;

    public Animator Animator {
        get {
            if (this.animator == null) {
                this.animator = GetComponent<Animator>();
            }
            return this.animator;
        }
        set => this.animator = value;
    }

    private Collider Collider {
        get {
            if (this.collider == null) {
                this.collider = GetComponent<Collider>();
            }
            return this.collider;
        }
        set => this.collider = value;
    }

    private AudioSource Audio {
        get {
            if (this.audio == null) {
                this.audio = GetComponent<AudioSource>();
            }
            return this.audio;
        }
        set => this.audio = value;
    }

    private ParticleSystem[] ParticleSystems {
        get {
            if (this.particleSystems == null) {
                this.particleSystems = GetComponentsInChildren<ParticleSystem>();
            }
            return this.particleSystems;
        }
        set => this.particleSystems = value;
    }

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
        this.sightAnimator = this.sight.GetComponent<Animator>();
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
        this.Audio.PlayOneShot(this.shieldCloseClip);
        this.Animator.SetBool("IsOpen", false);
        this.Collider.enabled = true;
        this.Collider.isTrigger = true;
    }

    private void OpenShield() {
        this.Audio.PlayOneShot(this.shieldOpenClip);
        this.Animator.SetBool("IsOpen", true);
        this.Collider.enabled = false;
        this.Collider.isTrigger = false;
    }

    private IEnumerator EmitFire() {
        foreach (var particleSystem in this.ParticleSystems) {
            particleSystem.Play();
        }
        yield return new WaitForSeconds(0.1f);
        foreach (var particleSystem in this.ParticleSystems) {
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
        MakeShot();
    }
}
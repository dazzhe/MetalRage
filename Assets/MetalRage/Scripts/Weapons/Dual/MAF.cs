using System.Collections;
using UnityEngine;

public class MAF : Weapon {
    [SerializeField]
    private AudioClip shieldOpenClip;
    [SerializeField]
    private AudioClip shieldCloseClip;
    [SerializeField]
    private Transform shotOrigin;

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
        this.Ammo.MagazineSize = 900;
        this.Ammo.ReserveBulletCount = 900;
        this.Ammo.Reload();
        this.param.damage = 13;
        this.param.recoil = Vector2.zero;
        this.Spread.MinAngle = 0f;
        this.Spread.GrowRate = 0f;
        this.ray = new WeaponRay(Camera.main, this.shotOrigin, 25f);
        this.param.reloadTime = 1.5f;
        this.param.coolDownTime = 0.06f;
        Initialize();
        this.sightAnimator = this.Crosshair.GetComponent<Animator>();
    }

    private void Update() {
        StartCoroutine(ShotControl());
        if (this.robot.UserCommand.Fire2) {
            this.photonView.RPC("Shield", PhotonTargets.AllBuffered);
        }
    }

    protected IEnumerator ShotControl() {
        var canShot = !this.Ammo.IsMagazineEmpty && !this.param.isCoolDown && this.isOpen && !this.param.isReloading;
        if (this.robot.UserCommand.Fire1 && canShot) {
            this.sightAnimator.SetBool("rotate", true);
            var hit = this.ray.Raycast(Vector2.zero);
            if (GameManager.IsEnemy(hit.collider)) {
                hit.collider.GetComponent<Hit>().TakeDamage(this.param.damage, this.unitMotor.name);
            }
            RecoilAndSpread();
            ConsumeBullets(2);
            this.photonView.RPC("MakeShots", PhotonTargets.All);
            this.param.isCoolDown = true;
            yield return new WaitForSeconds(this.param.coolDownTime);
            this.param.isCoolDown = false;
        } else if (this.robot.UserCommand.Fire1 != true) {
            this.sightAnimator.SetBool("rotate", false);
        }
    }

    public override void Unselect() {
        if (!this.isOpen) {
            this.photonView.RPC("Shield", PhotonTargets.AllBuffered);
        }

        base.Unselect();
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
            CloseShield();
        } else {
            this.isOpen = true;
            OpenShield();
        }
    }

    [PunRPC]
    protected void MakeShots() {
        MakeShot();
    }
}
using System.Collections;
using UnityEngine;

/// <summary>
/// Base class of weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour {
    public class Parameters {
        public int damage = 0;
        public Vector2 recoil;
        public Vector2 maxRecoil = new Vector2(3f, 14f);
        public float reloadTime = 0f;
        public float coolDownTime = 0f;
        public float setupTime = 1f;
        public bool isReloading = false;
        public bool isCoolDown = false;
    }

    [SerializeField]
    protected GameObject crosshairPrefab = default;
    [SerializeField]
    private Ammo ammo = default;
    [SerializeField]
    private BulletSpread spread = default;
    [SerializeField]
    private AudioClip setupClip = default;
    [SerializeField]
    private AudioClip reloadClip = default;

    protected Parameters param = new Parameters();
    protected WeaponRay ray;
    protected PhotonView photonView;
    protected Robot robot;
    protected UnitMotor unitMotor;
    protected new AudioSource audio;

    public Robot Robot { get; set; }
    public Crosshair Crosshair { get; protected set; }
    public Ammo Ammo => this.ammo;
    public BulletSpread Spread => this.spread;
    public float SensitivityScale { get; protected set; } = 1f;

    protected virtual void OnDestroy() {
        if (this.photonView.isMine && this.Crosshair != null) {
            Destroy(this.Crosshair.gameObject);
        }
    }

    protected void Initialize() {
        this.photonView = this.Robot.GetComponent<PhotonView>();
        this.robot = this.Robot.GetComponent<Robot>();
        this.unitMotor = this.Robot.GetComponent<UnitMotor>();
        this.audio = GetComponent<AudioSource>();
        if (this.photonView.isMine && this.crosshairPrefab != null) {
            var crosshairObj = Instantiate(this.crosshairPrefab, Vector3.zero, Quaternion.identity);
            this.Crosshair = crosshairObj.GetComponentInChildren<Crosshair>();
            this.Crosshair.Hide();
        }
    }

    protected void RecoilAndSpread() {
        StopCoroutine(Recoil());
        StartCoroutine(Recoil());
        this.spread.Spread();
    }

    private IEnumerator Recoil() {
        this.robot.IsRecoiling = true;
        float nextRecoilRotY;
        float nextRecoilRotX;
        float desiredRecoilY = this.param.recoil.y * (1f + this.robot.Dispersion);
        float desiredRecoilX = this.param.recoil.x * (1f + this.robot.Dispersion);
        if (this.robot.RecoilRotation.y <= this.param.maxRecoil.y - 1f) {
            nextRecoilRotY = this.robot.RecoilRotation.y + desiredRecoilY;
        } else {
            nextRecoilRotY = NextRecoilInRange(this.param.maxRecoil.y - 1f,
                                               this.param.maxRecoil.y,
                                               this.robot.RecoilRotation.y,
                                               0.5f);
        }
        nextRecoilRotX = NextRecoilInRange(-this.param.maxRecoil.x, this.param.maxRecoil.x,
                                           this.robot.RecoilRotation.x,
                                           desiredRecoilX);
        for (int i = 0; i < 7; ++i) {
            this.robot.RecoilRotation = new Vector2 {
                x = Mathf.Lerp(this.robot.RecoilRotation.x, nextRecoilRotX, 50f * Time.deltaTime),
                y = Mathf.Lerp(this.robot.RecoilRotation.y, nextRecoilRotY, 50f * Time.deltaTime)
            };
            yield return null;
        }
        this.robot.IsRecoiling = false;
    }

    private float NextRecoilInRange(float min, float max, float origin, float range) {
        float recoil = Random.Range(-range, range);
        return (min < origin + recoil && origin + recoil < max) ? origin + recoil : origin - recoil;
    }

    protected void ConsumeBullets(int count) {
        this.Ammo.Consume(count);
        if (this.Ammo.CanReload && this.Ammo.LoadedBulletCount == 0) {
            StartCoroutine(ReloadRoutine());
        }
    }

    protected virtual IEnumerator ReloadRoutine() {
        this.audio.PlayOneShot(this.reloadClip);
        this.param.isReloading = true;
        yield return new WaitForSeconds(this.param.reloadTime);
        this.Ammo.Reload();
        this.param.isReloading = false;
    }

    public virtual void Select() {
        this.audio.PlayOneShot(this.setupClip);
        this.Crosshair?.Show();
        StopCoroutine(SelectRoutine());
        StartCoroutine(SelectRoutine());
    }

    public virtual void Unselect() {
        this.robot.IsRecoiling = false;
        this.param.isCoolDown = false;
        StopAllCoroutines();
        this.audio.Stop();
        InterruptReloading();
        this.enabled = false;
        if (this.Crosshair != null) {
            this.Crosshair.Hide();
        }
    }

    protected IEnumerator SelectRoutine() {
        yield return new WaitForSeconds(this.param.setupTime);
        this.enabled = true;
    }

    private void InterruptReloading() {
        this.param.isReloading = false;
    }
}

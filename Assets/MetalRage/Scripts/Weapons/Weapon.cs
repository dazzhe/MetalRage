using System.Collections;
using UnityEngine;

public class WeaponParam {
    public int damage = 0;
    public float recoilY = 0;
    public float recoilX = 0;
    public float maxRecoilY = 14f;
    public float maxRecoilX = 3f;
    public float minDispersion = 0;
    public float dispersionGrow = 0;
    public float maxrange = 0;
    public float reloadTime = 0;
    public float interval = 0;
    public float setupTime = 1f;
    public bool isReloading;
    public bool cooldown = false;
}

public class WeaponComponent {
    public GameObject unit;
    public AudioSource setup;
    public AudioSource reloadAudio;
    public PhotonView myPV;
    public WeaponControl wcontrol;
    public UnitMotor motor;
}

/// <summary>
/// Base class of weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour {
    [SerializeField]
    private GameObject gunsightPrefab;
    [SerializeField]
    private Ammo ammo;

    protected WeaponParam param = new WeaponParam();
    protected WeaponComponent component = new WeaponComponent();
    public Crosshair Crosshair { get; protected set; }

    public GameObject GunsightPrefab => this.gunsightPrefab;
    public Ammo Ammo => this.ammo;

    protected virtual void OnDestroy() {
        if (this.component.myPV.isMine && this.Crosshair != null) {
            Destroy(this.Crosshair.gameObject);
        }
    }

    protected void Init() {
        this.component.unit = this.transform.parent.parent.parent.gameObject;
        this.component.myPV = GetComponent<PhotonView>();
        this.component.wcontrol = this.component.unit.GetComponent<WeaponControl>();
        this.component.motor = this.component.unit.GetComponent<UnitMotor>();
        if (GetComponent<AudioSource>()) {
            AudioSource[] audioSources = GetComponents<AudioSource>();
            this.component.setup = audioSources[0];
            if (audioSources.Length > 1) {
                this.component.reloadAudio = audioSources[1];
            }
        }
        if (this.component.myPV.isMine && this.GunsightPrefab != null) {
            var crosshairObj = Instantiate(this.GunsightPrefab, Vector3.zero, Quaternion.identity);
            this.Crosshair = crosshairObj.GetComponentInChildren<Crosshair>();
            this.Crosshair.Hide();
        }
    }

    protected void RecoilAndDisperse() {
        StopCoroutine(Recoil());
        StartCoroutine(Recoil());
        if (this.component.wcontrol.DispersionRate < 3f) {
            this.component.wcontrol.DispersionRate += this.param.dispersionGrow;
        } else {
            this.component.wcontrol.DispersionRate = 3f;
        }
    }

    private IEnumerator Recoil() {
        this.component.wcontrol.IsRecoiling = true;
        float nextRecoilRotY;
        float nextRecoilRotX;
        float desiredRecoilY = this.param.recoilY * (1f + this.component.wcontrol.Dispersion);
        float desiredRecoilX = this.param.recoilX * (1f + this.component.wcontrol.Dispersion);
        if (this.component.wcontrol.RecoilRotation.y <= this.param.maxRecoilY - 1f) {
            nextRecoilRotY = this.component.wcontrol.RecoilRotation.y + desiredRecoilY;
        } else {
            nextRecoilRotY = NextRecoilInRange(this.param.maxRecoilY - 1f,
                                               this.param.maxRecoilY,
                                               this.component.wcontrol.RecoilRotation.y,
                                               0.5f);
        }
        nextRecoilRotX = NextRecoilInRange(-this.param.maxRecoilX, this.param.maxRecoilX,
                                           this.component.wcontrol.RecoilRotation.x,
                                           desiredRecoilX);
        int i = 0;
        while (i <= 6) {
            this.component.wcontrol.RecoilRotation = new Vector2 {
                x = Mathf.Lerp(this.component.wcontrol.RecoilRotation.x, nextRecoilRotX, 50f * Time.deltaTime),
                y = Mathf.Lerp(this.component.wcontrol.RecoilRotation.y, nextRecoilRotY, 50f * Time.deltaTime)
            };
            i++;
            yield return null;
        }
        this.component.wcontrol.IsRecoiling = false;
    }

    private float NextRecoilInRange(float min, float max, float origin, float range) {
        float recoil = Random.Range(-range, range);
        return (min < origin + recoil && origin + recoil < max) ? origin + recoil : origin - recoil;
    }

    protected void ConsumeBullets(int count) {
        this.Ammo.Consume(count);
        if (this.Ammo.CanReload && this.Ammo.LoadedBulletCount == 0) {
            StartCoroutine(Reload());
        }
    }

    protected virtual IEnumerator Reload() {
        this.component.reloadAudio.PlayOneShot(this.component.reloadAudio.clip);
        this.param.isReloading = true;
        yield return new WaitForSeconds(this.param.reloadTime);
        this.Ammo.Reload();
        this.param.isReloading = false;
    }

    public virtual void Select() {
        this.component.setup.PlayOneShot(this.component.setup.clip);
        this.Crosshair?.Show();
        StopCoroutine(SelectRoutine());
        StartCoroutine(SelectRoutine());
    }

    public virtual void Unselect() {
        this.component.wcontrol.IsRecoiling = false;
        this.param.cooldown = false;
        StopAllCoroutines();
        this.component.setup.Stop();
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

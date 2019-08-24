using System.Collections;
using UnityEngine;

public class WeaponParam {
    public int ammo = 0;
    public int magazine = 0;
    public int damage = 0;
    public float recoilY = 0;//反動.
    public float recoilX = 0;
    public float maxRecoilY = 14f;
    public float maxRecoilX = 3f;
    public float minDispersion = 0;//ばらつき.
    public float dispersionGrow = 0;
    public float maxrange = 0;
    public float reloadTime = 0;
    public float interval = 0;
    public float setupTime = 1f;
    public bool isReloading;
    public int load;
    public bool cooldown = false;
    public bool canShot;
}

public class WeaponComponent {
    public GameObject unit;
    public AudioSource setup;
    public AudioSource reload;
    public PhotonView myPV;
    public WeaponControl wcontrol;
    public UnitMotor motor;
}

//Base class of weapons
public abstract class Weapon : MonoBehaviour {
    protected WeaponParam param = new WeaponParam();
    protected WeaponComponent component = new WeaponComponent();
    protected GameObject sightObject;
    protected Sight sight;
    protected string sightPrefabName = "";

    protected void Init() {
        this.component.unit = this.transform.parent.parent.parent.gameObject;
        this.component.myPV = GetComponent<PhotonView>();
        this.component.wcontrol = this.component.unit.GetComponent<WeaponControl>();
        this.component.motor = this.component.unit.GetComponent<UnitMotor>();
        if (GetComponent<AudioSource>()) {
            AudioSource[] audioSources = GetComponents<AudioSource>();
            this.component.setup = audioSources[0];
            if (audioSources.Length > 1) {
                this.component.reload = audioSources[1];
            }
        }
        this.param.load = this.param.magazine;
        this.param.canShot = true;
        if (this.component.myPV.isMine && this.sightPrefabName != "") {
            this.sightObject = GameObject.Instantiate(Resources.Load(this.sightPrefabName), Vector3.zero, Quaternion.identity) as GameObject;
            this.sight = this.sightObject.GetComponentInChildren<Sight>();
            this.sight.HideSight();
        }
    }

    protected void RecoilAndDisperse() {
        StopCoroutine("Recoil");
        StartCoroutine("Recoil");
        if (this.component.wcontrol.dispersionRate < 3f) {
            this.component.wcontrol.dispersionRate += this.param.dispersionGrow;
        } else {
            this.component.wcontrol.dispersionRate = 3f;
        }
    }

    private IEnumerator Recoil() {
        this.component.wcontrol.isRecoiling = true;
        float nextRecoilRotY;
        float nextRecoilRotX;
        float desiredRecoilY = this.param.recoilY * (1f + this.component.wcontrol.desiredDispersion);
        float desiredRecoilX = this.param.recoilX * (1f + this.component.wcontrol.desiredDispersion);
        if (this.component.wcontrol.recoilrotationy <= this.param.maxRecoilY - 1f) {
            nextRecoilRotY = this.component.wcontrol.recoilrotationy + desiredRecoilY;
        } else {
            nextRecoilRotY = NextRecoilInRange(this.param.maxRecoilY - 1f,
                                               this.param.maxRecoilY,
                                               this.component.wcontrol.recoilrotationy,
                                               0.5f);
        }
        nextRecoilRotX = NextRecoilInRange(-this.param.maxRecoilX, this.param.maxRecoilX,
                                           this.component.wcontrol.recoilrotationx,
                                           desiredRecoilX);
        int i = 0;
        while (i <= 6) {
            this.component.wcontrol.recoilrotationx = Mathf.Lerp(this.component.wcontrol.recoilrotationx, nextRecoilRotX, 50f * Time.deltaTime);
            this.component.wcontrol.recoilrotationy = Mathf.Lerp(this.component.wcontrol.recoilrotationy, nextRecoilRotY, 50f * Time.deltaTime);
            i++;
            yield return null;
        }
        this.component.wcontrol.isRecoiling = false;
    }

    private float NextRecoilInRange(float min, float max, float origin, float range) {
        float recoil = Random.Range(-range, range);
        return (min < origin + recoil && origin + recoil < max) ? origin + recoil : origin - recoil;
    }

    protected void RemainingLoads(int b) {
        this.param.load = Mathf.Clamp(this.param.load - b, 0, this.param.magazine);
        if (this.param.load == 0 && this.param.ammo != 0) {
            StartCoroutine(Reload());
        }
        UIManager.Instance.StatusUI.SetMagazine(this.param.load);
    }

    protected virtual IEnumerator Reload() {
        this.component.reload.PlayOneShot(this.component.reload.clip);
        this.param.isReloading = true;
        yield return new WaitForSeconds(this.param.reloadTime);
        int supplyLoad = Mathf.Min(this.param.magazine - this.param.load, this.param.ammo);
        this.param.load += supplyLoad;
        this.param.ammo -= supplyLoad;
        this.param.isReloading = false;
        UIManager.Instance.StatusUI.SetMagazine(this.param.load);
        UIManager.Instance.StatusUI.SetAmmo(this.param.ammo);
    }

    protected virtual void Enable() {
        UIManager.Instance.StatusUI.SetMagazine(this.param.load);
        UIManager.Instance.StatusUI.SetAmmo(this.param.ammo);
        this.component.setup.PlayOneShot(this.component.setup.clip);
        if (this.sight != null) {
            this.sight.ShowSight();
        }
        StopCoroutine("EnableCoroutine");
        StartCoroutine("EnableCoroutine");
    }

    protected IEnumerator EnableCoroutine() {
        yield return new WaitForSeconds(this.param.setupTime);
        this.enabled = true;
    }

    protected virtual void OnDestroy() {
        if (this.component.myPV.isMine && this.sightObject != null) {
            Destroy(this.sightObject);
        }
    }

    protected virtual void Disable() {
        this.component.wcontrol.isRecoiling = false;
        this.param.cooldown = false;
        StopAllCoroutines();
        this.component.setup.Stop();
        interruptReloading();
        this.enabled = false;
        if (this.sight != null) {
            this.sight.HideSight();
        }
    }

    private void interruptReloading() {
        this.param.isReloading = false;
    }
}
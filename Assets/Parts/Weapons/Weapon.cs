using UnityEngine;
using System.Collections;

public class WeaponParam
{
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

public class WeaponComponent
{
	public GameObject unit;
	public AudioSource reload;
	public PhotonView myPV;
	public WeaponControl wcontrol;
	public NormalDisplay normdisp;
	public UnitMotor motor;
}

//Base class of weapons
public abstract class Weapon : MonoBehaviour
{
	protected WeaponParam param = new WeaponParam();
	protected WeaponComponent component = new WeaponComponent();
	protected GameObject sightObject;
	protected Sight sight;
	protected string sightPrefabName = "";

	protected void Init()
	{
		component.unit = transform.parent.parent.parent.gameObject;
		component.myPV = GetComponent<PhotonView>();
		component.wcontrol = component.unit.GetComponent<WeaponControl>();
		component.motor = component.unit.GetComponent<UnitMotor>();
		if (GetComponent<AudioSource>()) {
			AudioSource[] audioSources = GetComponents<AudioSource>();
			component.reload = audioSources[0];
		}
		param.load = param.magazine;
		param.canShot = true;
		if (component.myPV.isMine && sightPrefabName != "") {
			sightObject = GameObject.Instantiate(Resources.Load(sightPrefabName), Vector3.zero, Quaternion.identity) as GameObject;
			sight = sightObject.GetComponentInChildren<Sight>();
			sight.HideSight();
		}
	}

	protected void RecoilAndDisperse()
	{
		StopCoroutine("Recoil");
		StartCoroutine("Recoil");
		if (component.wcontrol.dispersionRate < 3f) {
			component.wcontrol.dispersionRate += param.dispersionGrow;
		} else {
			component.wcontrol.dispersionRate = 3f;
		}
	}

	private IEnumerator Recoil()
	{
		component.wcontrol.isRecoiling = true;
		float nextRecoilRotY;
		float nextRecoilRotX;
		float desiredRecoilY = param.recoilY * (1f + component.wcontrol.desiredDispersion);
		float desiredRecoilX = param.recoilX * (1f + component.wcontrol.desiredDispersion);
		if (component.wcontrol.recoilrotationy <= param.maxRecoilY - 1f) {
			nextRecoilRotY = component.wcontrol.recoilrotationy + desiredRecoilY;
		} else {
			nextRecoilRotY = NextRecoilInRange(param.maxRecoilY - 1f,
			                                   param.maxRecoilY,
			                                   component.wcontrol.recoilrotationy,
			                                   0.5f);
		}
		nextRecoilRotX = NextRecoilInRange(-param.maxRecoilX, param.maxRecoilX,
		                                   component.wcontrol.recoilrotationx,
		                                   desiredRecoilX);
		int i = 0;
		while (i <= 6) {
			component.wcontrol.recoilrotationx = Mathf.Lerp(component.wcontrol.recoilrotationx, nextRecoilRotX, 50f * Time.deltaTime);
			component.wcontrol.recoilrotationy = Mathf.Lerp(component.wcontrol.recoilrotationy, nextRecoilRotY, 50f * Time.deltaTime);
			i++;
			yield return null;
		}
		component.wcontrol.isRecoiling = false;
	}

	private float NextRecoilInRange(float min, float max, float origin, float range)
	{
		float recoil = Random.Range(-range, range);
		return (min < origin + recoil && origin + recoil < max) ? origin + recoil : origin - recoil;
	}
	
	protected void RemainingLoads(int b)
	{
		param.load = Mathf.Clamp(param.load - b, 0, param.magazine);
		if (param.load == 0 && param.ammo != 0) {
			StartCoroutine(this.Reload());
		}
		NormalDisplay.SetMagazine(param.load);
	}
	
	protected virtual IEnumerator Reload()
	{
		component.reload.PlayOneShot(component.reload.clip);
		param.isReloading = true;
		yield return new WaitForSeconds(param.reloadTime);
		int supplyLoad = Mathf.Min(param.magazine - param.load, param.ammo);
		param.load += supplyLoad;
		param.ammo -= supplyLoad;
		param.isReloading = false;
		NormalDisplay.SetMagazine(param.load);
		NormalDisplay.SetAmmo(param.ammo);
	}

	protected virtual void Enable()
	{
		NormalDisplay.SetMagazine(param.load);
		NormalDisplay.SetAmmo(param.ammo);
		if (sight != null) {
			sight.ShowSight();
		}
		StopCoroutine("EnableCoroutine");
		StartCoroutine("EnableCoroutine");
	}

	protected IEnumerator EnableCoroutine()
	{
		yield return new WaitForSeconds(param.setupTime);
		this.enabled = true;
	}

	protected virtual void OnDestroy()
	{
		if (component.myPV.isMine && sightObject != null) {
			Destroy(sightObject);
		}
	}

	protected virtual void Disable()
	{
		component.wcontrol.isRecoiling = false;
		param.cooldown = false;
		StopAllCoroutines();
		interruptReloading();
		this.enabled = false;
		if (sight != null) {
			sight.HideSight();
		}
	}

	void interruptReloading()
	{
		param.isReloading = false;
	}
}
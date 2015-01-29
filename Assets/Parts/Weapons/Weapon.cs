using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour{
	private AudioSource reload;
	protected PhotonView myPV;
	protected WeaponControl wcontrol;
	protected NormalDisplay normdisp;
	protected UnitMotor motor;
	[SerializeField]
	protected int maxload = 0;
	protected int damage = 0;
	protected float recoil = 0;//反動.
	protected float mindispersion = 0;//ばらつき.
	protected float dispersiongrow = 0;
	protected float maxrange = 0;
	protected float reloadTime = 0;
	protected float interval = 0;
	protected bool isReloading;
	protected bool canShot;
	protected int load;

	protected bool cooldown = false;

	protected void Init () {
		myPV = GetComponent<PhotonView>();
		wcontrol = GetComponentInParent<WeaponControl>();
		motor = GetComponentInParent<UnitMotor>();
		AudioSource[] audioSources = GetComponents<AudioSource>();
		reload = audioSources[0];
		load = maxload;
		canShot = true;
		NormalDisplay.SetReticle(wcontrol.dispersionRate * mindispersion);
		normdisp = GameObject.Find ("NormalDisplay").GetComponent<NormalDisplay>();
		normdisp.NOLtext.text = load.ToString();
		if (myPV.isMine)
			this.enabled = true;
	}

	protected void RecoilAndDisperse(){
		StopCoroutine("Recoil");
		StartCoroutine("Recoil");
		if (wcontrol.dispersionRate < 3f)
			wcontrol.dispersionRate += dispersiongrow;
		else
			wcontrol.dispersionRate = 3f;
	}

	private IEnumerator Recoil(){
		wcontrol.isRecoiling = true;
		float nextRecoilRotY;
		float nextRecoilRotX;
		if (wcontrol.recoilrotationy < 14F)
			nextRecoilRotY = wcontrol.recoilrotationy + recoil * (1f + wcontrol.desiredDispersion);
		else
			nextRecoilRotY = wcontrol.recoilrotationy - 1F;
		nextRecoilRotX = wcontrol.recoilrotationx + Random.Range(-recoil,recoil);
		int i = 0;
		while(i <= 6){
			wcontrol.recoilrotationx = Mathf.Lerp(wcontrol.recoilrotationx, nextRecoilRotX, 50f * Time.deltaTime);
			wcontrol.recoilrotationy = Mathf.Lerp(wcontrol.recoilrotationy, nextRecoilRotY, 50f * Time.deltaTime);
			i++;
			yield return null;
		}
		wcontrol.isRecoiling = false;
	}

	protected IEnumerator HitMark(){
		NormalDisplay.RedReticle();
		yield return new WaitForSeconds(0.3f);
		NormalDisplay.WhiteReticle();
	}
	
	protected void RemainingLoads(int b){
		load -= b;
		if (load == 0){
			StartCoroutine(this.Reload ());
		}
		normdisp.NOLtext.text = load.ToString();
	}
	
	protected IEnumerator Reload(){
		reload.PlayOneShot(reload.clip);
		isReloading = true;
		yield return new WaitForSeconds(reloadTime);
		load = maxload;
		isReloading = false;
		normdisp.NOLtext.text = load.ToString();
	}
}
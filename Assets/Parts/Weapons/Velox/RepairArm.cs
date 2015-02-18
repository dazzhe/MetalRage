using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RepairArm : Weapon {
	private Animation _animation;
	AudioSource repair;
	Text HP;
	Status s;
	void Awake () {
		param.magazine = 100;
		param.damage = -40;
		param.recoil = 0f;//反動.
		param.minDispersion = 0f;//ばらつき.
		param.dispersionGrow = 0f;
		param.maxrange = 1000;
		_animation = GetComponent<Animation>();
		Init ();
		AudioSource[] audioSources = GetComponents<AudioSource>();
		repair = audioSources[0];
		HP = GameObject.Find("NormalDisplay/HP").GetComponent<Text>();
		if (component.myPV.isMine){
			StartCoroutine("Remain");
		}
	}
	IEnumerator Remain(){
		while(true){
			if (param.load < param.magazine && _animation.IsPlaying("Default")){
				param.load += 2;
				if (param.load > param.magazine)
					param.load = param.magazine;
				NormalDisplay.SetMagazine(param.load);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	void LateUpdate () {
		s = component.wcontrol.targetObject.GetComponent<Status>();
		if (s != null)
			HP.text = s.HP.ToString();
		else
			HP.text = "";
		if (component.wcontrol.inputShot1){
			if (_animation.IsPlaying("Default"))
				_animation.CrossFade("PreRepair",0.5f);
			else if (!_animation.IsPlaying("PreRepair") && !_animation.IsPlaying("Repair"))
				component.myPV.RPC ("RepairRPC",PhotonTargets.All);
		}
		else {
			if (!component.myPV.isMine && _animation.IsPlaying("Default"))
				component.myPV.RPC ("Default",PhotonTargets.All);
			else
				_animation.Play("Default");
		}
	}

	void Repair(){
		repair.PlayOneShot(repair.clip);
		if (component.myPV.isMine){
			Hit h = component.wcontrol.targetObject.GetComponentInParent<Hit>();
			if (h != null && component.wcontrol.targetObject.layer == 9 && s.HP != s.maxHP && param.load >= 15
			    && (component.wcontrol.targetObject.transform.position
			   	  - transform.position).magnitude <= 4f){
				h.TakeDamage(param.damage,component.unit.name);
				RemainingLoads(15);
			}
		}
	}

	[RPC]
	void Default(){
		_animation.Play("Default");
	}

	[RPC]
	void RepairRPC(){
		_animation.Play ("Repair");
	}
}
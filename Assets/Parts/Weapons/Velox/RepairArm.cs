using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RepairArm : Weapon {
	private Animation _animation;
	AudioSource repair;
	Text HP;
	Status s;
	void Awake () {
		maxload = 100;
		damage = -40;
		recoil = 0f;//反動.
		mindispersion = 0f;//ばらつき.
		dispersiongrow = 0f;
		maxrange = 1000;
		_animation = GetComponent<Animation>();
		Init ();
		AudioSource[] audioSources = GetComponents<AudioSource>();
		repair = audioSources[0];
		HP = GameObject.Find("NormalDisplay/HP").GetComponent<Text>();
		if (myPV.isMine){
			StartCoroutine("Remain");
			NormalDisplay.DeleteReticle();
		}
	}
	IEnumerator Remain(){
		while(true){
			if (load < maxload && _animation.IsPlaying("Default")){
				load += 2;
				if (load > maxload)
					load = maxload;
				normdisp.NOLtext.text = load.ToString();
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	void LateUpdate () {
		s = wcontrol.targetObject.GetComponent<Status>();
		if (s != null)
			HP.text = s.HP.ToString();
		else
			HP.text = "";
		if (wcontrol.inputShot1){
			if (_animation.IsPlaying("Default"))
				_animation.CrossFade("PreRepair",0.5f);
			else if (!_animation.IsPlaying("PreRepair") && !_animation.IsPlaying("Repair"))
				myPV.RPC ("RepairRPC",PhotonTargets.All);
		}
		else {
			if (!myPV.isMine && _animation.IsPlaying("Default"))
				myPV.RPC ("Default",PhotonTargets.All);
			else
				_animation.Play("Default");
		}
	}

	void Repair(){
		repair.PlayOneShot(repair.clip);
		if (myPV.isMine){
			Hit h = wcontrol.targetObject.GetComponentInParent<Hit>();
			if (h != null && wcontrol.targetObject.layer == 9 && s.HP != s.maxHP && load >= 15
			    && (wcontrol.targetObject.transform.position
			   	  - transform.position).magnitude <= 4f){
				h.TakeDamage(damage,unit);
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
	void OnDestroy(){
		if (Camera.main != null)
			Camera.main.fieldOfView = 90;
		NormalDisplay.EnableReticle();
	}
}
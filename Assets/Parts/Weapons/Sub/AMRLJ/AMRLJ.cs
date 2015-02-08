using UnityEngine;
using System.Collections;

public class AMRLJ : Weapon {
	private Animation _animation;
	AudioSource repair;
	Status s;
	void Awake () {
		param.ammo = 4;
		param.magazine = 4;
		param.recoil = 0f;
		param.mindispersion = 0f;
		param.dispersiongrow = 0f;
		param.interval = 1.5f;
		Init ();
	}

	void LateUpdate () {
		if(component.wcontrol.inputShot1 && param.load != 0 && !param.cooldown && param.canShot)
			StartCoroutine("Shot");
	}

	IEnumerator Shot(){
		param.cooldown = true;
		for (int i = 0; i <= 1; i++){
			if (param.load == 0)
				yield break;
			MakeShot(component.wcontrol.targetPos);
			RecoilAndDisperse ();
			RemainingLoads(1);
			if (i == 1) break;
			yield return new WaitForSeconds(0.2f);
		}
		yield return new WaitForSeconds(param.interval);
		param.cooldown = false;
	}

	void MakeShot(Vector3 targetPos){
		StartCoroutine("CreateBullet",targetPos);
	}

	IEnumerator CreateBullet(Vector3 targetPos){
		Quaternion rot = Quaternion.LookRotation(targetPos - transform.position);
		GameObject rocket = PhotonNetwork.Instantiate("Rocket",transform.position, rot, 0) as GameObject;
		Rocket _rocket = rocket.GetComponent<Rocket>();
		_rocket.shooterPlayer = component.unit.name;
		_rocket.wcontrol = component.wcontrol;
		yield return null;
	}
}
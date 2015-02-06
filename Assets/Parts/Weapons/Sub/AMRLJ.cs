using UnityEngine;
using System.Collections;

public class AMRLJ : Weapon {
	private Animation _animation;
	AudioSource repair;
	Status s;
	void Awake () {
		param.magazine = 4;
		param.recoil = 0f;
		param.mindispersion = 0f;
		param.dispersiongrow = 0f;
		Init ();
	}

	void LateUpdate () {
		if(component.wcontrol.inputShot1 && param.load != 0)
			StartCoroutine("Shot");
	}

	IEnumerator Shot(){
		component.myPV.RPC("MakeShot",PhotonTargets.All,component.wcontrol.targetPos);
		if (param.load != 0){
			yield return new WaitForSeconds(0.2f);
			component.myPV.RPC("MakeShot",PhotonTargets.All,component.wcontrol.targetPos);
		}
	}

	[RPC]
	void MakeShot(Vector3 targetPos){
		StartCoroutine("CreateBullet",targetPos);
	}

	IEnumerator CreateBullet(Vector3 targetPos){
		GameObject bul = GameObject.Instantiate(Resources.Load("AMRL-J_Rocket"),transform.position,transform.rotation) as GameObject;
		Bullet _bul = bul.GetComponent<Bullet>();
		_bul.targetPos = targetPos;
		_bul.originPos = transform.position;
		yield return null;
	}
}
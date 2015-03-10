using UnityEngine;
using System.Collections;

public class AMRLJ : Weapon
{
	Animation anim;
	void Awake ()
	{
		param.magazine = 4;
		param.recoilY = 0f;
		param.minDispersion = 10f;	//only for showing sight
		param.dispersionGrow = 3f;
		param.interval = 1.5f;
		sightPrefabName = "Rocket_Sight";
		anim = GetComponent<Animation>();
		Init ();
	}

	void LateUpdate ()
	{
		if(component.wcontrol.inputShot1 && param.load != 0 && !param.cooldown && param.canShot)
			StartCoroutine("Shot");
		sight.extent = param.minDispersion * component.wcontrol.desiredDispersion * 2;
	}

	IEnumerator Shot()
	{
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

	void MakeShot(Vector3 targetPos)
	{
		StartCoroutine("CreateBullet",targetPos);
	}

	IEnumerator CreateBullet(Vector3 targetPos)
	{
		Quaternion rot = Quaternion.LookRotation(targetPos - transform.position);
		GameObject rocket = PhotonNetwork.Instantiate("Rocket",transform.parent.position, rot, 0) as GameObject;
		Rocket _rocket = rocket.GetComponent<Rocket>();
		_rocket.shooterPlayer = component.unit.name;
		_rocket.wcontrol = component.wcontrol;
		yield return null;
	}

	protected override void Enable()
	{
		anim["SettingUp"].speed = 1;
		anim.Play("SettingUp");
		base.Enable();
	}

	protected override void Disable ()
	{
		if (anim["SettingUp"].time == 0){
			anim["SettingUp"].time = anim["SettingUp"].length;
		}
		anim["SettingUp"].speed = -3;
		anim.Play ("SettingUp");
		base.Disable ();
	}

}
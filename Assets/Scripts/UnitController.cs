using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {
	GameObject normdisp;

	private static string[] pkse = {"","01_shot","02_double_kill","03_mass_kill","04_multi_kill",
								"05_crazy_kill","06_great_kill","07_excellent","08_massacre",
								"09_super_action","10_unbellevable","11_fantastic","12_bigbang",
								"13_holy_shot","14_holy_shot_haha"};

	WeaponControl weaponctrl;
	UnitMotor motor;
	NormalDisplay normaldisplay;
	Status stat;

	GUIText noltext;
	
	private int killcount;

	void Start () {
		normdisp = GameObject.Find ("NormalDisplay");
		weaponctrl = GetComponent<WeaponControl>();
		motor = GetComponent<UnitMotor>();
		stat = GetComponent<Status>();
		normaldisplay = normdisp.GetComponent<NormalDisplay>();
		GameObject nolbar = GameObject.Find("NOLBar");
		noltext = nolbar.GetComponent<GUIText>();
	}

	void Update () {
		motor.inputMoveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		motor.inputRotationX = Input.GetAxis("Mouse X") * Configuration.sensitivity;
		motor.inputJump = Input.GetButtonDown ("Jump");
		motor.inputBoost = Input.GetButtonDown("Boost");
		motor.inputSquat = Input.GetButton("Squat");

		normaldisplay.HPtext.text = stat.HP.ToString();
		normaldisplay.HPBar.value = 1f * stat.HP / stat.maxHP;
		noltext.text = weaponctrl.load.ToString();
		normaldisplay.SetBoostGauge(motor.boostgauge);
	}

	void LateUpdate(){
		if (stat.HP == 0){
			normaldisplay.DeathCount();
			GameObject go = GameObject.Find ("RandomMatchmaker");
			go.GetComponent<GameManager>().Die(transform.position, this.gameObject);
		}
	}

	[RPC]
	void OnKilledPlayer(){
		if (GetComponent<PhotonView>().isMine){
			killcount++;
			normaldisplay.KillCount();
			if (killcount > 14)
				SoundPlayer.Instance.PlaySE(pkse[14]);
			else
				SoundPlayer.Instance.PlaySE(pkse[killcount]);
		}
	}
}

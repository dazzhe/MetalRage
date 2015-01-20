using UnityEngine;
using System.Collections;
//このスクリプトは自分の機体についているときのみenabledになる.
public class UnitController : MonoBehaviour {
	GameObject normdisp;
	private static string[] pkSE
		= {"","01_shot","02_double_kill","03_mass_kill","04_multi_kill",
		"05_crazy_kill","06_great_kill","07_excellent","08_massacre",
		"09_super_action","10_unbellevable","11_fantastic","12_bigbang",
		"13_holy_shot","14_holy_shot_haha"};

	WeaponControl weaponctrl;
	UnitMotor motor;
	NormalDisplay normaldisplay;
	Status stat;
	Canvas settings;
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
		settings = GameObject.Find("Settings").GetComponent<Canvas>();
	}

	void Update () {
		//トラフィックを減らすために１バイトの変数に入力情報をまとめている.
		byte b = 0;
		if (!settings.enabled){
			motor.rotationX
				= transform.localEulerAngles.y
				+ Input.GetAxis("Mouse X") * Configuration.sensitivity * motor.sensimag;
			if (Input.GetAxisRaw("Horizontal") == 1)
				b += 64;
			else if (Input.GetAxisRaw("Horizontal") == -1)	
				b += 192;
			if (Input.GetAxisRaw("Vertical") == 1)
				b += 16;	
			else if (Input.GetAxisRaw("Vertical") == -1)
				b += 48;
			if (Input.GetButtonDown ("Jump"))
				b += 8;
			if (Input.GetButtonDown("Boost"))
				b += 4;
			if (Input.GetButton("Squat"))
				b += 2;
			if (!Screen.lockCursor || Screen.showCursor){
				Screen.lockCursor = true;
				Screen.showCursor = false;
			}
		}
		//入力情報を同期させる.
		if (motor.inputState != b){
			GetComponent<PhotonView>().RPC("InputState",PhotonTargets.All, b);
		}
		normaldisplay.HPtext.text = stat.HP.ToString();
		normaldisplay.HPBar.value = 1f * stat.HP / stat.maxHP;
		noltext.text = weaponctrl.load.ToString();
		normaldisplay.SetBoostGauge(motor.boostgauge);
	}

	[RPC]
	public void InputState(byte inputState){
		GetComponent<UnitMotor>().inputState = inputState;
	}
	
	void LateUpdate(){
		if (stat.HP == 0){
			normaldisplay.DeathCount();
			GameObject go = GameObject.Find ("RandomMatchmaker");
			go.GetComponent<GameManager>().Die(transform.position, this.gameObject);
		}
	}

	//自分の攻撃によって死んだプレイヤーが実行する関数であるため.
	//RPC関数となっている.
	[RPC]
	void OnKilledPlayer(){
		if (GetComponent<PhotonView>().isMine){
			killcount++;
			normaldisplay.KillCount();
			if (killcount > 14)
				SoundPlayer.Instance.PlaySE(pkSE[14]);
			else
				SoundPlayer.Instance.PlaySE(pkSE[killcount]);
		}
	}
}

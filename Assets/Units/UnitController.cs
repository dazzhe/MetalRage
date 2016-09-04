using UnityEngine;
using System.Collections;
//縺薙?繧ｹ繧ｯ繝ｪ繝励ヨ縺ｯ閾ｪ蛻??讖滉ｽ薙↓縺､縺?※縺?ｋ縺ｨ縺阪?縺ｿenabled縺ｫ縺ｪ繧?
public class UnitController : MonoBehaviour {
	private static string[] pkSE
		= {"","01_shot","02_double_kill","03_mass_kill","04_multi_kill",
		"05_crazy_kill","06_great_kill","07_excellent","08_massacre",
		"09_super_action","10_unbellevable","11_fantastic","12_bigbang",
		"13_holy_shot","14_holy_shot_haha"};

	UnitMotor motor;
	Status stat;
	
	private int killcount;

	void Start ()
	{
		motor = GetComponent<UnitMotor>();
		stat = GetComponent<Status>();
	}

	void Update ()
	{
		//繝医Λ繝輔ぅ繝?け繧呈ｸ帙ｉ縺吶◆繧√↓?代ヰ繧､繝医?螟画焚縺ｫ蜈･蜉帶ュ蝣ｱ繧偵∪縺ｨ繧√※縺?ｋ.
		byte b = 0;
		if (Menu.activeWindowLevel == 0){
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
			if (!Screen.lockCursor || Cursor.visible){
				Screen.lockCursor = true;
				Cursor.visible = false;
			}
		}
		//蜈･蜉帶ュ蝣ｱ繧貞酔譛溘＆縺帙ｋ.
		if (motor.inputState != b)
		{
			GetComponent<PhotonView>().RPC("InputState",PhotonTargets.All, b);
		}
		NormalDisplay.SetHP(stat.HP,stat.maxHP);
		NormalDisplay.SetBoostGauge(motor.boostgauge);
	}

	[RPC]
	public void InputState(byte inputState)
	{
		GetComponent<UnitMotor>().inputState = inputState;
	}
	
	void LateUpdate()
	{
		if (stat.HP == 0){
			ScoreBoard._myEntry.IncrementDeath();
			GameObject go = GameObject.Find ("GameManager");
			go.GetComponent<GameManager>().Die(transform.position, this.gameObject);
		}
	}

	//閾ｪ蛻??謾ｻ謦?↓繧医▲縺ｦ豁ｻ繧薙□繝励Ξ繧､繝､繝ｼ縺悟ｮ溯｡後☆繧矩未謨ｰ縺ｧ縺ゅｋ縺溘ａ.
	//RPC髢｢謨ｰ縺ｨ縺ｪ縺｣縺ｦ縺?ｋ.
	[RPC]
	void OnKilledPlayer()
	{
		if (GetComponent<PhotonView>().isMine){
			killcount++;
			ScoreBoard._myEntry.IncrementKill();
			if (killcount > 14) {
				SoundPlayer.Instance.PlaySE(pkSE[14]);
			} else {
				SoundPlayer.Instance.PlaySE(pkSE[killcount]);
			}
		}
	}
}
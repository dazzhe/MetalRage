using UnityEngine;

public class UnitController : MonoBehaviour {
    private static string[] killVoiceNames
        = {"","01_shot","02_double_kill","03_mass_kill","04_multi_kill",
        "05_crazy_kill","06_great_kill","07_excellent","08_massacre",
        "09_super_action","10_unbellevable","11_fantastic","12_bigbang",
        "13_holy_shot","14_holy_shot_haha"};
    private UnitMotor motor;
    private Status stat;

    private int killCount;

    private void Start() {
        this.motor = GetComponent<UnitMotor>();
        this.stat = GetComponent<Status>();
    }

    private void Update() {
        byte b = 0;
        if (UIManager.Instance.MenuUI.ActiveWindowLevel == 0) {
            this.motor.rotationX
                = this.transform.localEulerAngles.y
                + Input.GetAxis("Mouse X") * Configuration.sensitivity * this.motor.sensimag;
            if (Input.GetAxisRaw("Horizontal") == 1) {
                b += 64;
            } else if (Input.GetAxisRaw("Horizontal") == -1) {
                b += 192;
            }
            if (Input.GetAxisRaw("Vertical") == 1) {
                b += 16;
            } else if (Input.GetAxisRaw("Vertical") == -1) {
                b += 48;
            }
            if (Input.GetButtonDown("Jump")) {
                b += 8;
            }
            if (Input.GetButtonDown("Boost")) {
                b += 4;
            }
            if (Input.GetButton("Squat")) {
                b += 2;
            }
            if (Cursor.lockState == CursorLockMode.None || Cursor.visible) {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }
        }
        if (this.motor.inputState != b) {
            GetComponent<PhotonView>().RPC("InputState", PhotonTargets.All, b);
        }
        UIManager.Instance.StatusUI.SetHP(this.stat.HP, this.stat.maxHP);
        UIManager.Instance.StatusUI.SetBoostGauge(this.motor.boostgauge);
    }

    [PunRPC]
    public void InputState(byte inputState) {
        GetComponent<UnitMotor>().inputState = inputState;
    }

    private void LateUpdate() {
        if (this.stat.HP == 0) {
            ScoreBoardUI.myEntry.IncrementDeath();
            GameObject go = GameObject.Find("GameManager");
            go.GetComponent<GameManager>().Die(this.transform.position, this.gameObject);
        }
    }

    [PunRPC]
    private void OnKilledPlayer() {
        if (GetComponent<PhotonView>().isMine) {
            ++this.killCount;
            ScoreBoardUI.myEntry.IncrementKill();
            if (this.killCount > 14) {
                AudioManager.Instance.PlaySE(killVoiceNames[14]);
            } else {
                AudioManager.Instance.PlaySE(killVoiceNames[this.killCount]);
            }
        }
    }
}
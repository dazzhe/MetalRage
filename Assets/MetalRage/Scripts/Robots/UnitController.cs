using System.Linq;
using UnityEngine;

public class UnitController : MonoBehaviour {
    private UnitMotor motor;
    private Status stat;
    private int killStreakCount;

    private void Start() {
        this.motor = GetComponent<UnitMotor>();
        this.stat = GetComponent<Status>();
    }

    private void Update() {
        byte b = 0;
        if (UIManager.Instance.MenuUI.ActiveWindowLevel == 0) {
            this.motor.rotationX
                = this.transform.localEulerAngles.y
                + Input.GetAxis("Mouse X") * Configuration.Sensitivity.GetFloat() * this.motor.sensimag;
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
        }
        if (this.motor.inputState != b) {
            GetComponent<PhotonView>().RPC("InputState", PhotonTargets.All, b);
        }
        UIManager.Instance.StatusUI.SetHP(this.stat.HP, this.stat.MaxHP);
        UIManager.Instance.StatusUI.SetBoostGauge(this.motor.boostgauge);
    }

    [PunRPC]
    public void InputState(byte inputState) {
        GetComponent<UnitMotor>().inputState = inputState;
    }

    private void LateUpdate() {
        if (this.stat.HP == 0) {
            GameManager.Instance.KillLocalPlayer();
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    public void HandleKillPlayer(int attackerID) {
        GetComponent<PhotonView>().RPC("HandleKillPlayerOnNetwork", PhotonTargets.All, attackerID);
    }

    [PunRPC]
    private void HandleKillPlayerOnNetwork(int attackerID) {
        if (GetComponent<PhotonView>().owner.IsMasterClient) {
            var attackerTeam = (TeamColor)PhotonNetwork.playerList.First(p => p.ID == attackerID).CustomProperties["Team"];
            GameManager.Instance.GetTeam(attackerTeam).Score++;
        }
        if (GetComponent<PhotonView>().isMine) {
            ScoreboardUI.myEntry.IncrementKill();
            if (++this.killStreakCount > 14) {
                AudioManager.Instance.PlaySE(13);
            } else {
                AudioManager.Instance.PlaySE(this.killStreakCount - 1);
            }
        }
    }
}
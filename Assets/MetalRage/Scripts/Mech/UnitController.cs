using System.Linq;
using UnityEngine;

public class UnitController : MonoBehaviour {
    private Mech robot;
    private UnitMotor motor;
    private Status stat;
    private int killStreakCount;

    private void Awake() {
        this.robot = GetComponent<Mech>();
        this.motor = GetComponent<UnitMotor>();
        this.stat = GetComponent<Status>();
    }

    private void Update() {
        UIManager.Instance.StatusUI.SetHP(this.stat.HP, this.stat.MaxHP);
        UIManager.Instance.StatusUI.SetBoostGauge(this.motor.BoostGauge);
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
                SoundSystem.Instance.PlaySE(13);
            } else {
                SoundSystem.Instance.PlaySE(this.killStreakCount - 1);
            }
        }
    }
}
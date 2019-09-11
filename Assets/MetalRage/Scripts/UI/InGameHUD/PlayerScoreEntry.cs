using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreEntry : Photon.MonoBehaviour {
    [SerializeField]
    private Text playerNameText = default;
    [SerializeField]
    private Text killCountText = default;
    [SerializeField]
    private Text deathCountText = default;
    
    private int killCount = 0;
    private int deathCount = 0;
    public int team;

    private void Start() {
        if (this.photonView.isMine) {
            this.photonView.RPC("Init", PhotonTargets.AllBuffered, PhotonNetwork.playerName, this.team);
        }
    }

    private void Update() {
        this.killCountText.text = this.killCount.ToString();
        this.deathCountText.text = this.deathCount.ToString();
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(this.killCount);
            stream.SendNext(this.deathCount);
        } else {
            this.killCount = (int)stream.ReceiveNext();
            this.deathCount = (int)stream.ReceiveNext();
        }
    }

    public void IncrementKill() {
        PhotonNetwork.RPC(this.photonView, "IncrementKillCountOnNetwork", PhotonTargets.AllBuffered, true);
    }

    public void IncrementDeath() {
        PhotonNetwork.RPC(this.photonView, "IncrementDeathCountOnNetwork", PhotonTargets.AllBuffered, true);
    }

    public void AddScore() {
        ScoreboardUI.score[this.team] += this.killCount;
    }

    [PunRPC]
    private void Init(string name, int team) {
        GameObject scoreList = null;
        if (team == 0) {
            scoreList = UIManager.Instance.ScoreboardUI.RedScoreList;
        }
        if (team == 1) {
            scoreList = UIManager.Instance.ScoreboardUI.BlueScoreList;
        }
        this.transform.parent = scoreList.transform;
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        this.playerNameText.text = name;
        this.team = team;
        ScoreboardUI.startAdding = true;
    }

    [PunRPC]
    private void IncrementKillCountOnNetwork() {
        this.killCount++;
        this.killCountText.text = this.killCount.ToString();
    }

    [PunRPC]
    private void IncrementDeathCountOnNetwork() {
        this.deathCount++;
        this.deathCountText.text = this.deathCount.ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerInScoreboard : Photon.MonoBehaviour {
    public string playerName;
    public int playerID = 0;
    public int kill = 0;
    public int death = 0;
    public int team;
    private Text _kill;
    private Text _death;
    private PhotonView myPV;

    private void Start() {
        this.myPV = GetComponent<PhotonView>();
        this._kill = this.transform.Find("Kill").GetComponent<Text>();
        this._death = this.transform.Find("Death").GetComponent<Text>();
        if (this.myPV.isMine) {
            this.myPV.RPC("Init", PhotonTargets.AllBuffered, this.playerName, this.team);
        }
    }

    private void Update() {
        this._kill.text = this.kill.ToString();
        this._death.text = this.death.ToString();
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(this.kill);
            stream.SendNext(this.death);
        } else {
            this.kill = (int)stream.ReceiveNext();
            this.death = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void Init(string name, int t) {
        GameObject scoreList = null;
        if (t == 0) {
            scoreList = UIManager.Instance.ScoreBoardUI.RedScoreList;
        }

        if (t == 1) {
            scoreList = UIManager.Instance.ScoreBoardUI.BlueScoreList;
        }

        this.transform.parent = scoreList.transform;
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        Text _name = this.transform.Find("Name").GetComponent<Text>();
        _name.text = name;
        this.team = t;
        ScoreBoardUI.startAdding = true;
    }

    public void IncrementKill() {
        this.kill++;
        this._kill.text = this.kill.ToString();
    }

    public void IncrementDeath() {
        this.death++;
        this._death.text = this.death.ToString();
    }

    public void AddScore() {
        ScoreBoardUI.score[this.team] += this.kill;
    }
}

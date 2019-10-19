using System.Collections;
using UnityEngine;

public class FriendOrEnemy : MonoBehaviour {
    private PhotonView photonView;
    public TeamColor team = TeamColor.None;
    public string playerName;
    public int playerID;

    private void Start() {
        this.photonView = GetComponent<PhotonView>();
        if (this.photonView.isMine) {
            this.team = GameManager.Instance.PlayerTeam;
            this.playerName = PhotonNetwork.playerName;
            this.playerID = PhotonNetwork.player.ID;
            //We can't send variables of GameObject type so we name this unit for identifying
            this.name = PhotonNetwork.player.ID.ToString();
            this.photonView.RPC("Sync", PhotonTargets.OthersBuffered, this.team, this.playerName, this.playerID, this.name);
        }
        StartCoroutine(WaitAndProceed());
    }

    [PunRPC]
    private void Sync(TeamColor team, string playerName, int playerID, string objectName) {
        this.team = team;
        this.playerName = playerName;
        this.playerID = playerID;
        this.name = objectName;
    }

    // Wait until the player select team and recieve team of this unit
    private IEnumerator WaitAndProceed() {
        yield return new WaitUntil(() => GameManager.Instance.PlayerTeam != TeamColor.None);
        if (!this.photonView.isMine) {
            if (GameManager.Instance.PlayerTeam == this.team) {
                this.gameObject.SetLayerRecursively(9);//Teammate
            } else {
                this.gameObject.SetLayerRecursively(10);//Enemy
                this.enabled = false;
            }
        } else {
            this.enabled = false;
        }
    }

    private void OnGUI() {
        if (this.team != GameManager.Instance.PlayerTeam) {
            return;
        }
        var style = new GUIStyle {
            fontSize = 20
        };
        style.normal.textColor = Color.green;
        var nameTagPos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
        //Display nametag if the unit is ahead
        if (Vector3.Dot(this.gameObject.transform.position - Camera.main.transform.position,
                        Camera.main.transform.TransformDirection(Vector3.forward)) > 0) {
            GUI.Label(new Rect(nameTagPos.x,
                               Screen.height - nameTagPos.y,
                               100,
                               50),
                      this.playerName, style);
        }
    }
}
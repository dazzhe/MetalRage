using System.Collections;
using UnityEngine;

public class FriendOrEnemy : MonoBehaviour {
    private PhotonView photonView;
    public int team = 5;
    public string playerName;
    private Vector2 nameTagPos;
    private GUIStyle style;

    private void Start() {
        this.photonView = GetComponent<PhotonView>();
        this.style = new GUIStyle();
        if (this.photonView.isMine) {
            this.team = GameManager.PlayerTeam;
            this.playerName = PhotonNetwork.playerName;
            //We can't send variables of GameObject type so we name this unit for identifying
            this.name = PhotonNetwork.player.ID.ToString();
            this.photonView.RPC("Sync", PhotonTargets.OthersBuffered, this.team, this.playerName, this.name);
        }
        StartCoroutine(WaitAndProceed());
    }

    [PunRPC]
    private void Sync(int t, string n, string objectName) {
        this.name = objectName;
        this.playerName = n;
        this.team = t;
    }

    // Wait until the player select team and recieve team of this unit
    private IEnumerator WaitAndProceed() {
        yield return new WaitForSeconds(0.5f);// Somehow sometimes team becomes 0 only at first so...
        while (GameManager.PlayerTeam == 5 || this.team == 5) {
            yield return new WaitForSeconds(0.5f);
        }
        if (!this.photonView.isMine) {
            if (GameManager.PlayerTeam == this.team) {
                this.gameObject.SetLayerRecursively(9);//Teammate
                this.style.fontSize = 20;
                this.style.normal.textColor = Color.green;
            } else {
                this.gameObject.SetLayerRecursively(10);//Enemy
                this.enabled = false;
            }
        } else {
            this.enabled = false;
        }
    }

    private void OnGUI() {
        this.nameTagPos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
        //Display nametag if the unit is ahead
        if (Vector3.Dot(this.gameObject.transform.position - Camera.main.transform.position,
                        Camera.main.transform.TransformDirection(Vector3.forward)) > 0) {
            GUI.Label(new Rect(this.nameTagPos.x,
                               Screen.height - this.nameTagPos.y,
                               100,
                               50),
                      this.playerName, this.style);
        }
    }
}
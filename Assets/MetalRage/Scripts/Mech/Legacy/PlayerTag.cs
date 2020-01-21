//using System.Collections;
//using UnityEngine;

//public class PlayerTag : MonoBehaviour {
//    private PhotonView photonView;
//    public TeamColor team = TeamColor.None;
//    public string playerName;
//    public int playerID;

//    private void Start() {
//        this.photonView = GetComponent<PhotonView>();
//        if (this.photonView.isMine) {
//            this.playerName = PhotonNetwork.playerName;
//            this.playerID = PhotonNetwork.player.ID;
//            //We can't send variables of GameObject type so we name this unit for identifying
//            this.name = PhotonNetwork.player.ID.ToString();
//            this.photonView.RPC("Sync", PhotonTargets.OthersBuffered, this.team, this.playerName, this.playerID, this.name);
//        }
//    }

//    private void OnGUI() {
//        var style = new GUIStyle {
//            fontSize = 20
//        };
//        style.normal.textColor = Color.green;
//        var nameTagPos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
//        //Display nametag if the unit is ahead
//        if (Vector3.Dot(this.gameObject.transform.position - Camera.main.transform.position,
//                        Camera.main.transform.TransformDirection(Vector3.forward)) > 0) {
//            GUI.Label(new Rect(nameTagPos.x,
//                               Screen.height - nameTagPos.y,
//                               100,
//                               50),
//                      this.playerName, style);
//        }
//    }
//}
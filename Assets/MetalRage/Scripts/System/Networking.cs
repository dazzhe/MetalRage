//#pragma warning disable 0649

//using UnityEngine;
//using UnityEngine.UI;

//public class Networking : Photon.PunBehaviour {
//    [SerializeField]
//    private InputField playerNameInputField;
//    [SerializeField]
//    private GameObject inputNameWindow;

//    private int frameCount = 0;
//    private int fps;
//    private float prevTimestamp = 0f;

//    private void Start() {
//        PhotonNetwork.ConnectToRegion(CloudRegionCode.jp, "0.2");
//        PhotonNetwork.autoJoinLobby = true;
//        PhotonNetwork.sendRate = 30;
//    }

//    private void Update() {
//        ++this.frameCount;
//        if (Time.time > this.prevTimestamp + 1f) {
//            this.prevTimestamp = Time.time;
//            this.fps = this.frameCount;
//            this.frameCount = 0;
//        }
//    }

//    public override void OnConnectedToMaster() {
//        /* Automatically join lobby (autoJoinLobby = true) */
//    }

//    public override void OnJoinedLobby() {
//        this.inputNameWindow.SetActive(true);
//    }

//    private void OnPhotonRandomJoinFailed() {
//        PhotonNetwork.CreateRoom(null);
//    }

//    public override void OnJoinedRoom() {
//        UIManager.Instance.ScoreboardUI.SelectTeam();
//    }

//    // Call from GUI button
//    public void JoinRoom() {
//        PhotonNetwork.playerName = this.playerNameInputField.text;
//        this.inputNameWindow.SetActive(false);
//        PhotonNetwork.JoinRandomRoom();
//    }

//    private void OnGUI() {
//        var connectionStateString = PhotonNetwork.connectionStateDetailed.ToString();
//        if (connectionStateString == "Joined") {
//            GUILayout.Label($"Ping: {PhotonNetwork.GetPing()}\nFPS: {this.fps}");
//        } else {
//            GUILayout.Label(connectionStateString);
//        }
//    }
//}
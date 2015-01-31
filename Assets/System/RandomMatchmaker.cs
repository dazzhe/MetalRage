using UnityEngine;
using UnityEngine.UI;
public class RandomMatchmaker : Photon.MonoBehaviour{
    private PhotonView myPhotonView;
	public GameObject canvas;
    // Use this for initialization
    void Start(){
        PhotonNetwork.ConnectUsingSettings("0.1");
		PhotonNetwork.sendRate = 30;
	}

    void OnJoinedLobby(){

    }

    void OnPhotonRandomJoinFailed(){
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom(){
		ScoreBoard.SelectTeam();
		this.enabled = false;
    }

	public void JoinRoom(){
		InputField i = canvas.transform.FindChild("Image/InputField").GetComponent<InputField>();
		PhotonNetwork.playerName = i.text;
		Destroy (canvas);
		PhotonNetwork.JoinRandomRoom();
	}
	
	void OnGUI(){
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}

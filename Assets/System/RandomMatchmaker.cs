using UnityEngine;
using UnityEngine.UI;
public class RandomMatchmaker : Photon.MonoBehaviour{
    private PhotonView myPhotonView;
	private string _unit;
	UnitOption uo;
	public GameObject canvas;
	private string name;
    // Use this for initialization
    void Start(){
        PhotonNetwork.ConnectUsingSettings("0.1");
		PhotonNetwork.sendRate = 30;
		uo = GetComponent<UnitOption>();
	}

    void OnJoinedLobby(){

    }

    void OnPhotonRandomJoinFailed(){
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom(){
		ScoreBoard.AddPlayer(name);
		uo.enabled = true;
		this.enabled = false;
    }

	public void JoinRoom(){
		InputField i = canvas.transform.FindChild("Image/InputField").GetComponent<InputField>();
		name = i.text;
		PhotonNetwork.playerName = name;
		Destroy (canvas);
		PhotonNetwork.JoinRandomRoom();
	}
	
	void OnGUI(){
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}

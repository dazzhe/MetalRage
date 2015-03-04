using UnityEngine;
using UnityEngine.UI;
public class RandomMatchmaker : Photon.MonoBehaviour{
    private PhotonView myPhotonView;
	[SerializeField]
	private GameObject inputNameWindow;
    // Use this for initialization
    void Start()
	{
        PhotonNetwork.ConnectUsingSettings("0.1");
		PhotonNetwork.sendRate = 30;
	}

    void OnJoinedLobby(){
		inputNameWindow.SetActive(true);
    }

    void OnPhotonRandomJoinFailed()
	{
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
	{
		ScoreBoard.SelectTeam();
		this.enabled = false;
    }

	public void JoinRoom()
	{
		InputField i = inputNameWindow.transform.FindChild("Image/InputField").GetComponent<InputField>();
		PhotonNetwork.playerName = i.text;
		Destroy (inputNameWindow);
		PhotonNetwork.JoinRandomRoom();
	}
	
	void OnGUI()
	{
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}

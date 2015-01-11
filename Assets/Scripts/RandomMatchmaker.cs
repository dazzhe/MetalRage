using UnityEngine;

public class RandomMatchmaker : Photon.MonoBehaviour{
    private PhotonView myPhotonView;
	private string _unit;
	UnitOption uo;
    // Use this for initialization
    void Start(){
        PhotonNetwork.ConnectUsingSettings("0.1");
		uo = GetComponent<UnitOption>();
	}

    void OnJoinedLobby(){
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed(){
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom(){
		uo.enabled = true;
		this.enabled = false;
    }

	void Update(){
	}

    void OnGUI(){
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}

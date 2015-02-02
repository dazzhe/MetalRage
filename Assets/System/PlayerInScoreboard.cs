using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInScoreboard : Photon.MonoBehaviour {
	public string playerName;
	public int playerID = 0;
	public int kill = 0;
	public int death = 0;
	public int team;
	Text _kill;
	Text _death;
	PhotonView myPV;

	void Start () {
		myPV = GetComponent<PhotonView>();
		_kill = transform.FindChild("Kill").GetComponent<Text>();
		_death = transform.FindChild("Death").GetComponent<Text>();
		if (myPV.isMine)
			myPV.RPC("Init",PhotonTargets.AllBuffered,playerName,team);
	}

	void Update(){
		_kill.text = kill.ToString();
		_death.text = death.ToString();
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting){
			stream.SendNext(kill);
			stream.SendNext(death);
		}
		else{
			kill = (int)stream.ReceiveNext();
			death = (int)stream.ReceiveNext();
		}
	}
	[RPC]
	public void Init(string name, int t){
		GameObject scoreList = null;
		if (t == 0)
			scoreList = GameObject.Find("Scoreboard/Panel/REDPlayerScoreList");
		if (t == 1)
			scoreList = GameObject.Find("Scoreboard/Panel/BLUEPlayerScoreList");
		transform.parent = scoreList.transform;
		transform.localScale = new Vector3(1f,1f,1f);
		Text _name = transform.FindChild("Name").GetComponent<Text>();
		_name.text = name;
	}

	public void IncrementKill(){
		kill++;
		_kill.text = kill.ToString();
	}

	public void IncrementDeath(){
		death++;
		_death.text = death.ToString();
	}

	public void AddScore(){
		ScoreBoard.score[team] += kill;
	}
}

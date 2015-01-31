using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInScoreboard : MonoBehaviour {
	public string playerName;
	public int playerID = 0;
	public int kill = 0;
	public int death = 0;
	public int team;
	Text _kill;
	Text _death;
	PhotonView myPV;

	// Use this for initialization
	void Start () {
		myPV = GetComponent<PhotonView>();
		_kill = transform.FindChild("Kill").GetComponent<Text>();
		_death = transform.FindChild("Death").GetComponent<Text>();
		if (myPV.isMine)
			myPV.RPC("Init",PhotonTargets.AllBuffered,playerName,team);
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
		myPV.RPC("IncKill",PhotonTargets.AllBuffered);
	}
	[RPC]
	private void IncKill(){
		kill++;
		_kill.text = kill.ToString();
	}

	public void IncrementDeath(){
		myPV.RPC("IncDeath",PhotonTargets.AllBuffered);
	}
	[RPC]
	private void IncDeath(){
		death++;
		_death.text = death.ToString();
	}

	public void AddScore(){
		ScoreBoard.score[team] += kill;
	}
}

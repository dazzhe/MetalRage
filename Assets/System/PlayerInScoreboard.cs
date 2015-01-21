using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInScoreboard : MonoBehaviour {
	public string playerName;
	public int kill = 0;
	public int death = 0;
	Text _kill;
	Text _death;
	PhotonView myPV;

	// Use this for initialization
	void Start () {
		myPV = GetComponent<PhotonView>();
		_kill = transform.FindChild("Kill").GetComponent<Text>();
		_death = transform.FindChild("Death").GetComponent<Text>();
		if (myPV.isMine)
			myPV.RPC("Initialize",PhotonTargets.AllBuffered,playerName);
	}
	
	// Update is called once per frame
	void Update () {
		if (myPV.isMine){
			_death.text = death.ToString();
		}
	}

	[RPC]
	public void Initialize(string name){
		GameObject scoreList
			= GameObject.Find("Scoreboard/Panel/PlayerScoreList");
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
}

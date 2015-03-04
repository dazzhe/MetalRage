using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour{
	public static PlayerInScoreboard _myEntry;
	private static Canvas _canvas;
	public static bool startAdding = false;
	Text scoreR;
	Text scoreB;
	public static int[] score = {0,0};

	void Awake(){
		_canvas = GetComponent<Canvas>();
		scoreR = transform.FindChild("Panel/REDScore/Text").GetComponent<Text>();
		scoreB = transform.FindChild("Panel/BLUEScore/Text").GetComponent<Text>();
		StartCoroutine(Score());
	}

	//This will be called from RandomMatchmaker.OnJoinedRoom()
	public static void SelectTeam(){
		_canvas.enabled = true;
	}

	void Update(){
		if (Input.GetButton("ScoreBoard") && !_canvas.enabled)
			_canvas.enabled = true;
		else if (!Input.GetButton("ScoreBoard") && _canvas.enabled)
			_canvas.enabled = false;
	}

	//This will be called when the buttons on scoreboard are clicked

	public void AddPlayer(int team){
		GameManager.myTeam = team;
		GameObject myEntry
			= PhotonNetwork.Instantiate("PlayerScoreEntry",
			                            Vector3.zero,
			                            Quaternion.identity,0) as GameObject;
		_myEntry = myEntry.GetComponent<PlayerInScoreboard>();
		_myEntry.playerName = PhotonNetwork.playerName;
		_myEntry.team = team;
		UnitOption.UnitSelect();
		this.enabled = true;
	}

	//Calculate total score of the teams
	IEnumerator Score(){
		while(true){
			if (startAdding){
				score[0] = 0;
				score[1] = 0;
				transform.BroadcastMessage("AddScore");
				scoreR.text = score[0].ToString("D3");
				scoreB.text = score[1].ToString("D3");
			}
				yield return new WaitForSeconds(1.0f);
		}
	}
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour{
	public GameObject myEntry;
	public static PlayerInScoreboard _myEntry;
	private static Canvas _canvas;
	Text scoreR;
	Text scoreB;
	public static int[] score = {0,0};

	void Awake(){
		_canvas = GetComponent<Canvas>();
		scoreR = transform.FindChild("Panel/REDScore/Text").GetComponent<Text>();
		scoreB = transform.FindChild("Panel/BLUEScore/Text").GetComponent<Text>();
		StartCoroutine(Score());
	}

	public static void SelectTeam(){
		_canvas.enabled = true;
	}

	void Update(){
		if (Input.GetButton("ScoreBoard") && !_canvas.enabled)
			_canvas.enabled = true;
		else if (!Input.GetButton("ScoreBoard") && _canvas.enabled)
			_canvas.enabled = false;
	}

	public void AddPlayer(int team){
		GameManager.myTeam = team;
		myEntry
			= PhotonNetwork.Instantiate("PlayerScoreEntry",
			                            Vector3.zero,
			                            Quaternion.identity,0) as GameObject;
		_myEntry = myEntry.GetComponent<PlayerInScoreboard>();
		_myEntry.playerName = PhotonNetwork.playerName;
		_myEntry.team = team;
		UnitOption.UnitSelect();
		this.enabled = true;
	}

	IEnumerator Score(){
		while(true){
			score[0] = 0;
			score[1] = 0;
			transform.BroadcastMessage("AddScore");
			scoreR.text = score[0].ToString();
			scoreB.text = score[1].ToString();
			yield return new WaitForSeconds(1.0f);
		}
	}
}
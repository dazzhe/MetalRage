using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour{
	public static GameObject myEntry;
	public static PlayerInScoreboard _myEntry;
	Canvas _canvas;

	void Awake(){
		_canvas = GetComponent<Canvas>();
	}
	void Update(){
		if (Input.GetButton("ScoreBoard") && !_canvas.enabled)
			_canvas.enabled = true;
		else if (!Input.GetButton("ScoreBoard") && _canvas.enabled)
			_canvas.enabled = false;
	}

	public static void AddPlayer(string name){
		myEntry
			= PhotonNetwork.Instantiate("PlayerScoreEntry",
			                            Vector3.zero,
			                            Quaternion.identity,0) as GameObject;
		_myEntry = myEntry.GetComponent<PlayerInScoreboard>();
		_myEntry.playerName = name;
	}
}

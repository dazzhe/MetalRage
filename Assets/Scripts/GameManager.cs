using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Photon.MonoBehaviour {
	PhotonView myPV;
	GameObject explosion;
	private Vector3[] spawnPositions = new Vector3[]{
		new Vector3(10f, -5f, 5f), new Vector3(-92f, -20f, 6f),
		new Vector3(0f, -20f, 100f), new Vector3(-34f,11f,-12f),
		new Vector3(-34f, -24f,-108f), new Vector3(-100f, -24f, -150f)};
	/*
	struct PlayerData{
		public string playerName;
		public string playerID;
		public int HP;
		public int killCount;
		public int deathCount;
	}
	private List<PlayerData> PlayerList;
	*/

	// Use this for initialization
	void Start () {
		myPV = GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Spawn(string unit){
		GameObject player 
			= PhotonNetwork.Instantiate(unit,
			                            spawnPositions[Random.Range(0, 6)],
			                            Quaternion.identity, 0) as GameObject;
		GetComponent<UnitOption>().enabled = false;
		Screen.lockCursor = true;
		Screen.showCursor = false;
		player.layer = 8;
	}

	public void Die(Vector3 pos, GameObject go){
		PhotonNetwork.Destroy(go);
		PhotonNetwork.RPC(myPV, "Explosion", PhotonTargets.All,pos);
		GetComponent<UnitOption>().enabled = true;
		Screen.lockCursor = false;
		Screen.showCursor = true;
	}

	[RPC]
	void Explosion(Vector3 pos){
		Instantiate(Resources.Load("explosion"),pos,Quaternion.identity);
	}
}

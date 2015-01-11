using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Photon.MonoBehaviour {
	PhotonView myPV;
	GameObject explosion;
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
		GameObject player = PhotonNetwork.Instantiate(unit, Vector3.up * 3F + Random.Range (0F,20.0F) * Vector3.right, Quaternion.identity, 0) as GameObject;
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

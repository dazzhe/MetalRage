using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Photon.MonoBehaviour {
	PhotonView myPV;
	GameObject explosion;
	//respawnX[0]<-RespawnRangeX of RedTeam
	//respawnX[1]<-RespawnRangeX of BlueTeam...
	private float[,] respawnX = new float[2,2] {{-90, 27},{-90, 46}};
	private float[,] respawnZ = new float[2,2] {{120, 142},{-157, -143}};
	public static int myTeam = 5;	//<-if RedTeam then 0 else 1
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
	
	void Start () {
		myPV = GetComponent<PhotonView>();
	}

	public void Spawn(string unit){
		Vector3 spawnPos = new Vector3(Random.Range(respawnX[myTeam,0],respawnX[myTeam,1]),
		                               -22f,
		                               Random.Range(respawnZ[myTeam,0],respawnZ[myTeam,1]));
		GameObject player
			= PhotonNetwork.Instantiate(unit, spawnPos,
			                            Quaternion.identity, 0) as GameObject;
		GetComponent<UnitOption>().enabled = false;
		player.layer = 8;
		player.GetComponent<UnitController>().enabled = true;
		player.GetComponent<UnitMotor>().enabled = true;
		player.GetComponent<WeaponControl>().enabled = true;
		player.GetComponent<MainCamera>().enabled = true;
		FriendOrEnemy fn = player.GetComponent<FriendOrEnemy>();
		fn.team = myTeam;
		fn.playerName = PhotonNetwork.playerName;
	}

	public void Die(Vector3 pos, GameObject go){
		PhotonNetwork.Destroy(go);
		PhotonNetwork.RPC(myPV, "Explosion", PhotonTargets.All,pos);
		UnitOption.UnitSelect();
	}

	[RPC]
	void Explosion(Vector3 pos){
		Instantiate(Resources.Load("explosion"),pos,Quaternion.identity);
	}
}

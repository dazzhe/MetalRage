using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class Status :MonoBehaviour{
	public int maxHP;
	public float armor = 1f;

	[System.NonSerialized]
	public int HP;

	PhotonView myPV;
	private GameObject lastAttackedPlayer;

	void Awake(){
		myPV = GetComponent<PhotonView>();
		HP = maxHP;
	}

	//this will be called in local
	public void ReduceHP(int damage, string attacker){
		myPV.RPC("NetworkReduceHP", PhotonTargets.All, damage, attacker);
	}

	//Only an owner of this unit proceeds this
	[RPC]
	void NetworkReduceHP(int damage, string attacker){
		if (myPV.isMine && HP != 0){
			HP -= damage;
			HP = Mathf.Clamp(HP, 0, maxHP);
			myPV.RPC("SetHP",PhotonTargets.OthersBuffered,HP);
			if (HP == 0)
				GameObject.Find(attacker)
					.GetComponent<PhotonView>()
					.RPC ("OnKilledPlayer",PhotonTargets.Others);
		}
	}

	[RPC]
	void SetHP(int HP){
		this.HP = HP;
	}
}

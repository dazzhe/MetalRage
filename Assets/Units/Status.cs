using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class Status :MonoBehaviour{
	public int maxHP;
	public float armor = 1f;

	[System.NonSerialized]
	public int HP;

	private bool acceptDamage = true;
	PhotonView myPV;
	private GameObject lastAttackedPlayer;

	void Awake(){
		myPV = GetComponent<PhotonView>();
	}

	void Start(){
		HP = maxHP;
	}

	public void ReduceHP(int damage, GameObject attackedPlayer){
		if (acceptDamage){
			if (HP != 0)
				lastAttackedPlayer = attackedPlayer;
			myPV.RPC("NetworkReduceHP", PhotonTargets.All, damage);
			//複数のプレイヤーにキル判定が入らないように.
			//死んだとき最後に攻撃した機体のプレイヤーにキルしたときの処理を渡すようにする.
			if (HP == 0){
				lastAttackedPlayer.GetComponent<PhotonView>().RPC("OnKilledPlayer", PhotonTargets.All);
				acceptDamage = false;
			}
		}
	}

	[RPC]
	void NetworkReduceHP(int damage){
		HP -= damage;
		if (HP < 0)
			HP = 0;
	}

}

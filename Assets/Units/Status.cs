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
		HP = maxHP;
	}

	public void ReduceHP(int damage, GameObject attackedPlayer){
		if (acceptDamage){
			if (HP != 0)
				lastAttackedPlayer = attackedPlayer;
			myPV.RPC("NetworkReduceHP", PhotonTargets.All, damage);
			HP -= damage;
			HP = Mathf.Clamp(HP,0,maxHP);
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
		if (myPV.isMine){
			HP -= damage;
			if (HP < 0)
				HP = 0;
			if (HP > maxHP)
				HP = maxHP;
			myPV.RPC("SetHP",PhotonTargets.OthersBuffered,HP);
		}
	}
	[RPC]
	void SetHP(int HP){
		this.HP = HP;
	}
}

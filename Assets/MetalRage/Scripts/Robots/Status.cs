using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Status : MonoBehaviour {
    public int maxHP;
    public float armor = 1f;

    [System.NonSerialized]
    public int HP;
    private PhotonView myPV;
    private GameObject lastAttackedPlayer;

    private void Awake() {
        this.myPV = GetComponent<PhotonView>();
        this.HP = this.maxHP;
    }

    //this will be called in local
    public void ReduceHP(int damage, string attacker) {
        this.myPV.RPC("NetworkReduceHP", PhotonTargets.All, damage, attacker);
    }

    //Only an owner of this unit proceeds this
    [PunRPC]
    private void NetworkReduceHP(int damage, string attacker) {
        if (this.myPV.isMine && this.HP != 0) {
            this.HP -= damage;
            this.HP = Mathf.Clamp(this.HP, 0, this.maxHP);
            this.myPV.RPC("SetHP", PhotonTargets.OthersBuffered, this.HP);
            if (this.HP == 0) {
                GameObject.Find(attacker)
                    .GetComponent<PhotonView>()
                    .RPC("OnKilledPlayer", PhotonTargets.Others);
            }
        }
    }

    [PunRPC]
    private void SetHP(int HP) {
        this.HP = HP;
    }
}

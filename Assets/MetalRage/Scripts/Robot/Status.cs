using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Status : MonoBehaviour {
    [SerializeField]
    private int maxHP;
    [SerializeField]
    private float armor = 1f;
    [SerializeField]
    private GameObject explosionPrefab;

    public int HP { get; private set; }
    public int MaxHP { get => this.maxHP; set => this.maxHP = value; }
    public float Armor { get => this.armor; set => this.armor = value; }

    private PhotonView myPV;
    private GameObject lastAttackedPlayer;

    private void Awake() {
        this.myPV = GetComponent<PhotonView>();
        this.HP = this.MaxHP;
    }

    //this will be called in local
    public void ReduceHP(int damage, string attacker) {
        this.myPV.RPC("NetworkReduceHP", PhotonTargets.All, damage, attacker);
    }

    // Only an owner of this unit proceeds this
    [PunRPC]
    private void NetworkReduceHP(int damage, string attacker) {
        if (this.myPV.isMine && this.HP != 0) {
            this.HP -= damage;
            this.HP = Mathf.Clamp(this.HP, 0, this.MaxHP);
            this.myPV.RPC("SetHP", PhotonTargets.OthersBuffered, this.HP);
            if (this.HP == 0) {
                var attackerObj = GameObject.Find(attacker);
                var attackerID = attackerObj.GetComponent<FriendOrEnemy>().playerID;
                attackerObj.GetComponent<UnitController>().HandleKillPlayer(attackerID);
            }
        }
    }

    [PunRPC]
    private void SetHP(int value) {
        if (this.HP != 0 && value == 0) {
            Instantiate(this.explosionPrefab, this.transform.position, Quaternion.identity);
        }
        this.HP = value;
    }
}

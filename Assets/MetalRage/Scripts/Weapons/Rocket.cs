using UnityEngine;

public class Rocket : MonoBehaviour {
    [SerializeField]
    private GameObject explosionPrefab = default;

    private float maxSpeed = 200f;
    private float currentSpeed = 0f;

    [System.NonSerialized]
    public int damage = 100;

    public string shooterPlayer;
    public Mech wcontrol;
    private float radiusOfExplosion = 8f;
    private Vector3 direction;
    public Vector3 targetPos;
    private PhotonView photonView;

    private void Start() {
        this.direction = this.transform.forward;
        this.photonView = GetComponent<PhotonView>();
    }

    private void FixedUpdate() {
        this.currentSpeed = Mathf.Lerp(this.currentSpeed, this.maxSpeed, 5f * Time.deltaTime);
        GetComponent<Rigidbody>().velocity = this.currentSpeed * this.direction;
    }

    private void OnCollisionEnter(Collision col) {
        if (!this.photonView.isMine) {
            return;
        }
        Collider[] collidersInSphere = Physics.OverlapSphere(this.transform.position, this.radiusOfExplosion);
        foreach (Collider scol in collidersInSphere) {
            Hit dam = scol.gameObject.GetComponentInParent<Hit>();
            if (dam != null && (scol.gameObject.layer == 10 || scol.gameObject.layer == 8)) {

                float distance = Mathf.Clamp((this.transform.position - scol.gameObject.transform.position).magnitude,
                                             3f, this.radiusOfExplosion);
                //distance = 3 -> correctDamage = damage (distance between the rocket and surface is about 0)
                //distance = rOE -> correctDamage = damage * 0.2
                float correctDamage = this.damage
                    * (-0.8f / (this.radiusOfExplosion - 3f) * (distance - 3f) + 1f);
                dam.TakeDamage(Mathf.FloorToInt(correctDamage), this.shooterPlayer);
                this.wcontrol.HitMark();
            }
        }
        this.photonView.RPC("Explosion", PhotonTargets.All, this.transform.position);
        PhotonNetwork.Destroy(this.gameObject);
    }

    [PunRPC]
    private void Explosion(Vector3 position) {
        Instantiate(this.explosionPrefab, position, Quaternion.identity);
    }
}

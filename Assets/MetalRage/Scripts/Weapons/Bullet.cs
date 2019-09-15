using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField]
    private GameObject impactPrefab;
    [SerializeField]
    private GameObject impactOnEnemyPrefab;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float speed = 100f;
    [SerializeField]
    private float lifeTime = 0.5f;

    private PhotonView photonView;

    public GameObject Owner { get; set; }
    public Vector3 StartPosition { get; set; }
    public Vector3 EndPosition { get; set; }
    public Collider IgnoreCollider { get; set; }

    private void Awake() {
        this.photonView = GetComponent<PhotonView>();
    }

    private void Start() {
        StartCoroutine(MoveUntilCollideRoutine());
        Destroy(this.gameObject, this.lifeTime);
    }

    private IEnumerator MoveUntilCollideRoutine() {
        var velocity = (this.EndPosition - this.StartPosition).normalized * this.speed;
        var renderers = GetComponentsInChildren<MeshRenderer>();
        // Hide at the first frame.
        foreach (var renderer in renderers) {
            renderer.enabled = false;
        }
        yield return null;
        foreach (var renderer in renderers) {
            renderer.enabled = true;
        }
        while (true) {
            var nextPosition = this.transform.position + velocity * Time.deltaTime;
            if (SweepTest(this.transform.position, nextPosition, out RaycastHit hitInfo)) {
                nextPosition = hitInfo.point;
                var hasCollidedToRobot = hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
                    hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Teammate") ||
                    hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Player");
                if (hasCollidedToRobot) {
                    Instantiate(this.impactOnEnemyPrefab, hitInfo.point, this.transform.rotation);
                    if (this.damage > 0 && this.photonView.isMine) {
                        this.Owner.GetComponent<WeaponControl>().HitMark();
                        var attackerID = this.Owner.GetComponent<FriendOrEnemy>().name;
                        hitInfo.collider.gameObject.GetComponent<Status>().ReduceHP(this.damage, attackerID);
                    }
                } else {
                    Instantiate(this.impactPrefab, hitInfo.point, this.transform.rotation);
                }
                this.transform.position = nextPosition;
                yield return null;
                Destroy(this.gameObject);
                yield break;
            }
            this.transform.position = nextPosition;
            yield return null;
        }
    }

    private bool SweepTest(Vector3 currentPosition, Vector3 nextPosition, out RaycastHit hitInfo) {
        var distance = (nextPosition - currentPosition).magnitude;
        var direction = (nextPosition - currentPosition).normalized;
        var ray = new Ray(currentPosition, direction);
        return Physics.Raycast(ray, out hitInfo, distance)
            ? hitInfo.collider != this.IgnoreCollider
            : false;
    }
}

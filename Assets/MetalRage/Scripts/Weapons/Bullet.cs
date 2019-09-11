using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField]
    private GameObject impactPrefab;
    [SerializeField]
    private GameObject impactOnEnemyPrefab;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float lifeTime = 0.5f;

    public Vector3 originPos;
    public Vector3 targetPos;
    public Collider IgnoreCollider { get; set; }

    private void Start() {
        StartCoroutine(MoveUntilCollideRoutine());
        Destroy(this.gameObject, this.lifeTime);
    }

    private IEnumerator MoveUntilCollideRoutine() {
        var velocity = (this.targetPos - this.originPos).normalized * this.speed;
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
                var isPlayer = hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
                    hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Teammate") ||
                    hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Player");
                if (isPlayer) {
                    Instantiate(this.impactOnEnemyPrefab, hitInfo.point, this.transform.rotation);
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

using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField]
    private GameObject impact;
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
        while (true) {
            var nextPosition = this.transform.position + velocity * Time.deltaTime;
            if (SweepTest(this.transform.position, nextPosition, out RaycastHit hitInfo)) {
                Instantiate(this.impact, hitInfo.point, this.transform.rotation);
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

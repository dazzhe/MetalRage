using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public GameObject impact;
    public Vector3 originPos;
    public Vector3 targetPos;

    private void Start() {
        StartCoroutine(BulletMove());
    }

    private IEnumerator BulletMove() {
        this.transform.position = Vector3.Lerp(this.originPos, this.targetPos, 0.5f + Random.value * 0.2f);
        yield return new WaitForSeconds(0.07f);
        Instantiate(this.impact, this.targetPos, this.transform.rotation);
        Destroy(this.gameObject);
    }
}

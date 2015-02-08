using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public GameObject impact;
	public Vector3 originPos;
	public Vector3 targetPos;

	void Start () {
		StartCoroutine(this.BulletMove());
	}
	
	IEnumerator BulletMove(){
		transform.position = Vector3.Lerp(originPos, targetPos, 0.5f + Random.value * 0.2f);
		yield return new WaitForSeconds(0.07f);
		GameObject.Instantiate(impact,targetPos,transform.rotation);
		Destroy (gameObject);
		//yield return new WaitForSeconds(1f);
		//transform.position += direction.normalized * speed;
	}
}

using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public GameObject impact;
	private float speed = 10.0F;
	public Vector3 originPos;
	public Vector3 targetPos;
	public Vector3 direction;
	public int totalRenderingCount;
	public int renderingCount;
	// Use this for initialization
	void Start () {
		direction = targetPos - originPos;
		totalRenderingCount = Mathf.FloorToInt(direction.magnitude / speed) + 1;
		StartCoroutine(this.BulletMove());
	}
	
	IEnumerator BulletMove(){
		for (renderingCount = 0; renderingCount <= totalRenderingCount; renderingCount++){
			transform.position = originPos + ((float)renderingCount / totalRenderingCount) * direction * speed;
			yield return new WaitForSeconds(0.02f);
		}
		GameObject.Instantiate(impact,targetPos,transform.rotation);
		Destroy (gameObject);
		//yield return new WaitForSeconds(1f);
		//transform.position += direction.normalized * speed;
	}
}

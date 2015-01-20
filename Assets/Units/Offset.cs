using UnityEngine;
using System.Collections;
//足のアニメーションにあわせて上半身を動かす.
public class Offset : MonoBehaviour {
	public GameObject lb;
	private Vector3 offset;

	void Update () {
		offset = lb.transform.position;
		transform.position = offset;
	}
}

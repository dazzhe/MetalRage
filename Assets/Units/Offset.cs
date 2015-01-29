using UnityEngine;
using System.Collections;
//足のアニメーションにあわせて上半身を動かす.
public class Offset : MonoBehaviour {
	public GameObject lb;
	//private Vector3 originPos;

	//void Awake(){
	//	originPos = transform;
	//}

	void Update () {
		transform.localPosition = lb.transform.localPosition;
	}
}

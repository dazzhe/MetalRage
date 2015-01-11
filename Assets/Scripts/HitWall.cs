using UnityEngine;
using System.Collections;

public class HitWall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider coll){
		if ( coll.gameObject.tag == "BULLET"){
			Destroy(coll.gameObject);
		}
	}
}

using UnityEngine;
using System.Collections;

public class Offset : MonoBehaviour {
	public GameObject lb;
	private Vector3 offset;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		offset = lb.transform.position;
		transform.position = offset;
	}
}

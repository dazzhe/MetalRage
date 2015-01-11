using UnityEngine;
using System.Collections;

public class mawaru : MonoBehaviour {
	
	[System.NonSerialized]
	public float minimumY = -60F;
	
	[System.NonSerialized]
	public float maximumY = 60F;
	
	[System.NonSerialized]
	public float rotationY = 0F;
	
	[System.NonSerialized]
	public float normalrotationY = 0F;

	public float sensitivity = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		normalrotationY += Input.GetAxis ("Mouse Y") * sensitivity;
		normalrotationY = Mathf.Clamp (normalrotationY, minimumY, maximumY);
		rotationY = normalrotationY;
		transform.localEulerAngles = new Vector3 (-rotationY, 0, 0);
	}
}

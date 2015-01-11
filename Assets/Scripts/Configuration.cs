using UnityEngine;
using System.Collections;

public class Configuration : MonoBehaviour {
	public static float sensitivity = 2.0f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("sensitivity up")){
			sensitivity += 0.1f;
		}
		if (Input.GetButtonDown("sensitivity down")){
			sensitivity -= 0.1f;
		}
	}

	void OnGUI(){
		GUI.Label (new Rect (0, 0, 300, 60),"Sensitivity is "+sensitivity);
	}
}

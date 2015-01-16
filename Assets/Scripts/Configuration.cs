using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Configuration : MonoBehaviour {
	public static float sensitivity = 2.0f;
	public static float volume = 0.5f;
	Canvas _canvas;
	Slider sensiSL;
	Text sensiTX;
	Slider volSL;
	Text volTX;
	// Use this for initialization
	void Start () {
		_canvas = GetComponent<Canvas>();
		sensiSL = transform.FindChild("Sensitivity").GetComponent<Slider>();
		sensiTX = transform.FindChild("Sensitivity/SensiText").GetComponent<Text>();
		volSL = transform.FindChild("Volume").GetComponent<Slider>();
		volTX = transform.FindChild("Volume/VolumeText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Menu")){
			if (!_canvas.enabled)
				_canvas.enabled = true;
			else
				_canvas.enabled = false;
		}
		if (_canvas.enabled){
			if (Screen.lockCursor || !Screen.showCursor){
				Screen.lockCursor = false;
				Screen.showCursor = true;
			}
			sensitivity = sensiSL.value * 0.2f;
			sensiTX.text = sensiSL.value.ToString();
			AudioListener.volume = volSL.value * 0.02f;
			volTX.text = volSL.value.ToString();
		}
		/*if (Input.GetButtonDown("sensitivity up")){
			sensitivity += 0.1f;
		}
		if (Input.GetButtonDown("sensitivity down")){
			sensitivity -= 0.1f;
		}*/
	}

	/*void OnGUI(){
		GUI.Label (new Rect (0, 0, 300, 60),"Sensitivity is "+sensitivity);
	}*/
}

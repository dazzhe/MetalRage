using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Configuration : MonoBehaviour
{
	public static float sensitivity = 2.0f;
	public static float volume = 0.5f;
	Canvas canvas;
	Slider sensitivitySlider;
	Slider volumeSlider;
	Slider qualitySlider;
	Text sensitivityValue;
	Text volumeValue;
	Text qualityValue;

	// Use this for initialization
	void Start()
	{
		canvas = GetComponent<Canvas>();
		sensitivitySlider = transform.FindChild("Sensitivity").GetComponent<Slider>();
		sensitivityValue = transform.FindChild("Sensitivity/Value").GetComponent<Text>();
		volumeSlider = transform.FindChild("Volume").GetComponent<Slider>();
		volumeValue = transform.FindChild("Volume/Value").GetComponent<Text>();
		qualitySlider = transform.FindChild("Quality").GetComponent<Slider>();
		qualityValue = transform.FindChild("Quality/Value").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetButtonDown("Menu")) {
			ToggleCanvas();
		}

		if (canvas.enabled) {
			UpdateConfiguration();
		}
	}

	void ToggleCanvas()
	{
		canvas.enabled = !canvas.enabled;

		if (canvas.enabled) {
			ShowCursor();
		}
	}
	
	void ShowCursor()
	{
		if (Screen.lockCursor || !Screen.showCursor) {
			Screen.lockCursor = false;
			Screen.showCursor = true;
		}
	}

	void UpdateConfiguration()
	{
		sensitivity = sensitivitySlider.value * 0.2f;
		sensitivityValue.text = sensitivitySlider.value.ToString();
		AudioListener.volume = volumeSlider.value * 0.01f;
		volumeValue.text = volumeSlider.value.ToString();
		QualitySettings.SetQualityLevel((int)qualitySlider.value, true);
		qualityValue.text = qualitySlider.value.ToString();
	}
}

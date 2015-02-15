using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Configuration : MonoBehaviour
{
	public static float sensitivity = 2.0F;
	public static float volume = 0.5F;

	[SerializeField] Canvas canvas;
	[SerializeField] Slider sensitivitySlider;
	[SerializeField] Slider volumeSlider;
	[SerializeField] Slider qualitySlider;
	[SerializeField] Text sensitivityValue;
	[SerializeField] Text volumeValue;
	[SerializeField] Text qualityValue;

	public void UpdateSensitivity()
	{
		sensitivity = sensitivitySlider.value * 0.2F;
		sensitivityValue.text = sensitivitySlider.value.ToString();
	}
	
	public void UpdateSoundVolume()
	{
		AudioListener.volume = volumeSlider.value * 0.01F;
		volumeValue.text = volumeSlider.value.ToString();
	}
	
	public void UpdateQuality()
	{
		QualitySettings.SetQualityLevel((int)qualitySlider.value, true);
		qualityValue.text = qualitySlider.value.ToString();
	}

	// Use this for initialization
	void Start()
	{

	}
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetButtonDown("Menu")) {
			ToggleCanvas();
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
		Screen.lockCursor = false;
		Screen.showCursor = true;
	}
}

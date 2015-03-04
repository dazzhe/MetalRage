using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Configuration : MonoBehaviour
{
	public static float sensitivity;
	public static float volume;
	public static int quality;

	const string SensitivityKey = "ConfigurationSensitivity";
	const string SoundVolumeKey = "ConfigurationSoundVolume";
	const string QualityKey = "ConfigurationQuality";

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
		PlayerPrefs.SetFloat(SensitivityKey, sensitivity);
	}
	
	public void UpdateSoundVolume()
	{
		volume = volumeSlider.value * 0.01F;
		AudioListener.volume = volume;
		volumeValue.text = volumeSlider.value.ToString();
		PlayerPrefs.SetFloat(SoundVolumeKey, volume);
	}
	
	public void UpdateQuality()
	{
		quality = (int)qualitySlider.value;
		QualitySettings.SetQualityLevel(quality, true);
		qualityValue.text = qualitySlider.value.ToString();
		PlayerPrefs.SetInt(QualityKey, quality);
	}

	void Start()
	{
		InitializeSensitivity();
		InitializeSoundVolume();
		InitializeQuality();
	}

	void Update()
	{
		if (Input.GetButtonDown("Menu") && canvas.enabled) {
			ToggleCanvas();
		}
	}
	
	void InitializeSensitivity()
	{
		sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 2F);
		sensitivitySlider.value = sensitivity * 5F;
		sensitivityValue.text = sensitivitySlider.value.ToString();
	}

	void InitializeSoundVolume()
	{
		volume = PlayerPrefs.GetFloat(SoundVolumeKey, 0.5F);
		AudioListener.volume = volume;
		volumeSlider.value = volume * 100F;
		volumeValue.text = volumeSlider.value.ToString();
	}

	void InitializeQuality()
	{
		quality = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
		QualitySettings.SetQualityLevel(quality, true);
		qualitySlider.value = (float)quality;
		qualityValue.text = qualitySlider.value.ToString();
	}

	public void ToggleCanvas()
	{
		canvas.enabled = !canvas.enabled;

		if (canvas.enabled) {
			ShowCursor();
			Menu.activeWindowLevel = 2;
		} else {
			Menu.activeWindowLevel = 0;
		}
	}
	
	void ShowCursor()
	{
		Screen.lockCursor = false;
		Screen.showCursor = true;
	}
}

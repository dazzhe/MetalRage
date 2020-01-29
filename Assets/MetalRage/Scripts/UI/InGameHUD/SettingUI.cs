using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour {
    [SerializeField]
    private Slider sensitivitySlider = default;
    [SerializeField]
    private Slider volumeSlider = default;
    [SerializeField]
    private Slider qualitySlider = default;
    [SerializeField]
    private Text sensitivityValue = default;
    [SerializeField]
    private Text volumeValue = default;
    [SerializeField]
    private Text qualityValue = default;

    private void Start() {
        InitializeSensitivity();
        InitializeSoundVolume();
        InitializeQuality();
    }

    public void UpdateSensitivity() {
        Preferences.Sensitivity.SetFloat(this.sensitivitySlider.value * 0.2f);
        this.sensitivityValue.text = this.sensitivitySlider.value.ToString();
    }

    public void UpdateSoundVolume() {
        var newValue = this.volumeSlider.value * 0.01f;
        Preferences.Volume.SetFloat(newValue);
        AudioListener.volume = newValue;
        this.volumeValue.text = this.volumeSlider.value.ToString();
    }

    public void UpdateQuality() {
        var newValue = (int)this.qualitySlider.value;
        Preferences.Quality.SetInt(newValue);
        QualitySettings.SetQualityLevel(newValue, true);
        this.qualityValue.text = this.qualitySlider.value.ToString();
    }

    private void InitializeSensitivity() {
        this.sensitivitySlider.value = Preferences.Sensitivity.GetFloat() * 5f;
        this.sensitivityValue.text = this.sensitivitySlider.value.ToString();
    }

    private void InitializeSoundVolume() {
        var value = Preferences.Volume.GetFloat();
        AudioListener.volume = value;
        this.volumeSlider.value = value * 100f;
        this.volumeValue.text = this.volumeSlider.value.ToString();
    }

    private void InitializeQuality() {
        var value = Preferences.Quality.GetInt();
        QualitySettings.SetQualityLevel(value, true);
        this.qualitySlider.value = value;
        this.qualityValue.text = this.qualitySlider.value.ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour {
    [SerializeField]
    private Slider sensitivitySlider;
    [SerializeField]
    private Slider volumeSlider;
    [SerializeField]
    private Slider qualitySlider;
    [SerializeField]
    private Text sensitivityValue;
    [SerializeField]
    private Text volumeValue;
    [SerializeField]
    private Text qualityValue;

    private void Start() {
        InitializeSensitivity();
        InitializeSoundVolume();
        InitializeQuality();
    }

    public void UpdateSensitivity() {
        Configuration.Sensitivity.SetFloat(this.sensitivitySlider.value * 0.2f);
        this.sensitivityValue.text = this.sensitivitySlider.value.ToString();
    }

    public void UpdateSoundVolume() {
        var newValue = this.volumeSlider.value * 0.01f;
        Configuration.Volume.SetFloat(newValue);
        AudioListener.volume = newValue;
        this.volumeValue.text = this.volumeSlider.value.ToString();
    }

    public void UpdateQuality() {
        var newValue = (int)this.qualitySlider.value;
        Configuration.Quality.SetInt(newValue);
        QualitySettings.SetQualityLevel(newValue, true);
        this.qualityValue.text = this.qualitySlider.value.ToString();
    }

    private void InitializeSensitivity() {
        this.sensitivitySlider.value = Configuration.Sensitivity.GetFloat() * 5f;
        this.sensitivityValue.text = this.sensitivitySlider.value.ToString();
    }

    private void InitializeSoundVolume() {
        var value = Configuration.Volume.GetFloat();
        AudioListener.volume = value;
        this.volumeSlider.value = value * 100f;
        this.volumeValue.text = this.volumeSlider.value.ToString();
    }

    private void InitializeQuality() {
        var value = Configuration.Quality.GetInt();
        QualitySettings.SetQualityLevel(value, true);
        this.qualitySlider.value = value;
        this.qualityValue.text = this.qualitySlider.value.ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Configuration : MonoBehaviour {
    private static readonly string kSensitivityKey = "ConfigurationSensitivity";
    private static readonly string kSoundVolumeKey = "ConfigurationSoundVolume";
    private static readonly string kQualityKey = "ConfigurationQuality";

    [SerializeField]
    private Canvas canvas;
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

    public static float sensitivity;
    public static float volume;
    public static int quality;

    public void UpdateSensitivity() {
        sensitivity = this.sensitivitySlider.value * 0.2f;
        this.sensitivityValue.text = this.sensitivitySlider.value.ToString();
        PlayerPrefs.SetFloat(kSensitivityKey, sensitivity);
    }

    public void UpdateSoundVolume() {
        volume = this.volumeSlider.value * 0.01f;
        AudioListener.volume = volume;
        this.volumeValue.text = this.volumeSlider.value.ToString();
        PlayerPrefs.SetFloat(kSoundVolumeKey, volume);
    }

    public void UpdateQuality() {
        quality = (int)this.qualitySlider.value;
        QualitySettings.SetQualityLevel(quality, true);
        this.qualityValue.text = this.qualitySlider.value.ToString();
        PlayerPrefs.SetInt(kQualityKey, quality);
    }

    private void Start() {
        InitializeSensitivity();
        InitializeSoundVolume();
        InitializeQuality();
    }

    private void InitializeSensitivity() {
        sensitivity = PlayerPrefs.GetFloat(kSensitivityKey, 2F);
        this.sensitivitySlider.value = sensitivity * 5F;
        this.sensitivityValue.text = this.sensitivitySlider.value.ToString();
    }

    private void InitializeSoundVolume() {
        volume = PlayerPrefs.GetFloat(kSoundVolumeKey, 0.5F);
        AudioListener.volume = volume;
        this.volumeSlider.value = volume * 100F;
        this.volumeValue.text = this.volumeSlider.value.ToString();
    }

    private void InitializeQuality() {
        quality = PlayerPrefs.GetInt(kQualityKey, QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(quality, true);
        this.qualitySlider.value = quality;
        this.qualityValue.text = this.qualitySlider.value.ToString();
    }
}

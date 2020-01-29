using UnityEngine;

public class PrefValue {
    private readonly string key;
    private readonly float defaultFloatValue;
    private readonly int defaultIntValue;

    public PrefValue(string key, float defaultFloatValue) {
        this.key = key;
        this.defaultFloatValue = defaultFloatValue;
    }

    public PrefValue(string key, int defaultIntValue) {
        this.key = key;
        this.defaultIntValue = defaultIntValue;
    }

    public float GetFloat() {
        return PlayerPrefs.GetFloat(this.key, this.defaultFloatValue);
    }

    public void SetFloat(float value) {
        PlayerPrefs.SetFloat(this.key, value);
    }

    public int GetInt() {
        return PlayerPrefs.GetInt(this.key, this.defaultIntValue);
    }

    public void SetInt(int value) {
        PlayerPrefs.SetInt(this.key, value);
    }
}

public static class Preferences {
    public static PrefValue Sensitivity =>
        new PrefValue("ConfigurationSensitivity", 2f);
    public static PrefValue Volume =>
        new PrefValue("ConfigurationSoundVolume", 0.5f);
    public static PrefValue Quality =>
        new PrefValue("ConfigurationQuality", QualitySettings.GetQualityLevel());
}

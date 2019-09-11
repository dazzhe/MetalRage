using UnityEngine;

public class ConfigValue {
    private readonly string key;
    private readonly float defaultFloatValue;
    private readonly int defaultIntValue;

    public ConfigValue(string key, float defaultFloatValue) {
        this.key = key;
        this.defaultFloatValue = defaultFloatValue;
    }

    public ConfigValue(string key, int defaultIntValue) {
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

public static class Configuration {
    public static ConfigValue Sensitivity =>
        new ConfigValue("ConfigurationSensitivity", 2f);
    public static ConfigValue Volume =>
        new ConfigValue("ConfigurationSoundVolume", 0.5f);
    public static ConfigValue Quality =>
        new ConfigValue("ConfigurationQuality", QualitySettings.GetQualityLevel());
}

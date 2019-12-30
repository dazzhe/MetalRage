using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "MetalRage/Audio/SoundDef")]
public class SoundDefinition : ScriptableObject {
    [SerializeField]
    private AudioClip[] clips;
    [SerializeField]
    [Range(-60.0f, 0.0f)]
    private float volume = -6f;
    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float minDistance = 1.5f;
    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float distMax = 30f;
    [SerializeField]
    [Range(-20, 20.0f)]
    private float pitch = 0f;
    [SerializeField]
    [Range(0, 10)]
    private int loopCount = 1;
    [SerializeField]
    [Range(0, 10.0f)]
    private float delayMin = 0f;
    [SerializeField]
    [Range(0, 10.0f)]
    private float delayMax = 0f;
    [SerializeField]
    [Range(1, 20)]
    private int repeatMin = 1;
    [SerializeField]
    [Range(1, 20)]
    private int repeatMax = 1;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float spatialBlend = 1f;
    [Range(-1.0f, 1.0f)]
    private float panMin = 0f;
    [Range(-1.0f, 1.0f)]
    private float panMax = 0f;
    [SerializeField]
    private AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

    public float Volume { get => this.volume; set => this.volume = value; }

    public void OnValidate() {
        if (this.minDistance > distMax)
            minDistance = distMax;
        if (pitchMin > pitchMax)
            pitchMin = pitchMax;
        if (delayMin > delayMax)
            delayMin = delayMax;
        if (repeatMin > repeatMax)
            repeatMin = repeatMax;
        if (panMin > panMax)
            panMin = panMax;
    }
}

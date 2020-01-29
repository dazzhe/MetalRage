using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "MetalRage/Audio/SoundDefinition")]
public class SoundDefinition : ScriptableObject {
    [SerializeField]
    private AudioClip[] clips = default;
    [SerializeField]
    [Range(-60f, 0f)]
    private float volume = -6f;
    [SerializeField]
    private bool isLooping = false;
    [SerializeField]
    [Range(0.1f, 100f)]
    private float minDistance = 1.5f;
    [SerializeField]
    [Range(0.1f, 100f)]
    private float maxDistance = 30f;
    [SerializeField]
    [Range(-20f, 20f)]
    private float pitch = 0f;
    [SerializeField]
    [Range(0f, 1f)]
    private float spatialBlend = 1f;
    [SerializeField]
    private AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

    public float Volume { get => this.volume; set => this.volume = value; }
    public float MinDistance { get => this.minDistance; set => this.minDistance = value; }
    public float MaxDistance { get => this.maxDistance; set => this.maxDistance = value; }
    public float Pitch { get => this.pitch; set => this.pitch = value; }
    public float SpatialBlend { get => this.spatialBlend; set => this.spatialBlend = value; }
    public AudioRolloffMode RolloffMode { get => this.rolloffMode; set => this.rolloffMode = value; }
    public bool IsLooping { get => this.isLooping; set => this.isLooping = value; }

    public static SoundDefinition KillVoice =>
        Resources.Load<SoundDefinition>("KillVoice");

    public static SoundDefinition LeanSound =>
        Resources.Load<SoundDefinition>("LeanSound");

    public AudioClip[] GetClips() {
        return this.clips.Clone() as AudioClip[];
    }
}

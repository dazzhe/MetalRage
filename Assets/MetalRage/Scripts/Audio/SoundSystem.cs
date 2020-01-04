using UnityEngine;

public class SoundSystem {
    private AudioSource CreateAudioSource(SoundDefinition soundDefinition) {
        var obj = new GameObject("AudioSource");
        var audio = obj.AddComponent<AudioSource>();
        audio.clip = soundDefinition.GetClips()[0];
        audio.loop = soundDefinition.IsLooping;
        audio.spatialBlend = soundDefinition.SpatialBlend;
        return audio;
    }

    public void Play(SoundDefinition soundDefinition) {
        var audio = CreateAudioSource(soundDefinition);
        audio.Play();
    }

    public void Play(SoundDefinition soundDefinition, int index) {
        var audio = CreateAudioSource(soundDefinition);
        audio.PlayOneShot(soundDefinition.GetClips()[index]);
    }

    public void Play(SoundDefinition soundDefinition, Transform origin) {
        var audio = CreateAudioSource(soundDefinition);
        audio.transform.parent = origin;
        audio.Play();
    }
}

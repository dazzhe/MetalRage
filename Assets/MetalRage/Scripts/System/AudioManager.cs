using UnityEngine;

public class AudioManager : SingletonBehaviour<AudioManager> {
    [SerializeField]
    private AudioClip[] SEs = default;
    [SerializeField]
    private AudioClip leanClip = default;

    private new AudioSource audio;

    private void Start() {
        this.audio = GetComponent<AudioSource>();
    }

    public void PlaySE(int clipIndex) {
        this.audio.PlayOneShot(this.SEs[clipIndex]);
    }

    public void PlayLeanSE() {
        this.audio.PlayOneShot(this.leanClip);
    }
}

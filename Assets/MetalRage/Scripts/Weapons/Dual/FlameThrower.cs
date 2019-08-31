using UnityEngine;

public class FlameThrower : MonoBehaviour {
    [SerializeField]
    private new ParticleSystem particleSystem;
    [SerializeField]
    private new AudioSource audio;

    private void Awake() {
        if (this.particleSystem == null) {
            this.particleSystem = GetComponent<ParticleSystem>();
        }
        if (this.audio == null) {
            this.audio = GetComponent<AudioSource>();
        }
    }

    private void Update() {
        if (this.particleSystem.IsAlive() && !this.audio.isPlaying) {
            this.audio.Play();
        }
        if (!this.particleSystem.IsAlive() && this.audio.isPlaying) {
            this.audio.Stop();
        }
    }
}

using System.Collections;
using UnityEngine;

public class Engine : MonoBehaviour {
    [SerializeField]
    private ParticleSystem jetFlameParticle = default;

    private Coroutine showJetFrameRoutine;
    private new AudioSource audio;

    private void Awake() {
        this.audio = GetComponent<AudioSource>();
    }

    public void ShowJetFlame(float duration) {
        this.audio.Play();
        if (this.showJetFrameRoutine != null) {
            StopCoroutine(this.showJetFrameRoutine);
        }
        this.showJetFrameRoutine = StartCoroutine(ShowJetFlameRoutine(duration));
    }

    public void ShowJetFlame() {
        this.jetFlameParticle.Play();
    }

    public void HideJetFlame() {
        this.jetFlameParticle.Stop();
    }

    private IEnumerator ShowJetFlameRoutine(float duration) {
        ShowJetFlame();
        yield return new WaitForSeconds(duration);
        HideJetFlame();
    }
}

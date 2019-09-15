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

    public void ShowJetFlame(float duration, Vector3 direction) {
        this.audio.Play();
        if (this.showJetFrameRoutine != null) {
            StopCoroutine(this.showJetFrameRoutine);
        }
        this.showJetFrameRoutine = StartCoroutine(ShowJetFlameRoutine(duration, direction));
    }

    public void ShowJetFlame(Vector3 direction) {
        this.jetFlameParticle.transform.forward =
            this.jetFlameParticle.transform.parent.TransformDirection(direction);
        this.jetFlameParticle.Play();
    }

    public void HideJetFlame() {
        this.jetFlameParticle.Stop();
    }

    private IEnumerator ShowJetFlameRoutine(float duration, Vector3 direction) {
        ShowJetFlame(direction);
        yield return new WaitForSeconds(duration);
        HideJetFlame();
    }
}

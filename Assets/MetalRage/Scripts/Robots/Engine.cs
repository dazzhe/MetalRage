using System.Collections;
using UnityEngine;

public class Engine : MonoBehaviour {
    [SerializeField]
    private ParticleSystem[] jetFlameEffects = default;

    private Coroutine showJetFrameRoutine;

    public void ShowJetFlame(float duration) {
        if (this.showJetFrameRoutine != null) {
            StopCoroutine(this.showJetFrameRoutine);
        }
        this.showJetFrameRoutine = StartCoroutine(ShowJetFlameRoutine(duration));
    }

    public void ShowJetFlame() {
        foreach (var jetFlameEffect in this.jetFlameEffects) {
            jetFlameEffect.Play();
        }
    }

    public void HideJetFlame() {
        foreach (var jetFlameEffect in this.jetFlameEffects) {
            jetFlameEffect.Stop();
        }
    }

    private IEnumerator ShowJetFlameRoutine(float duration) {
        ShowJetFlame();
        yield return new WaitForSeconds(duration);
        HideJetFlame();
    }
}

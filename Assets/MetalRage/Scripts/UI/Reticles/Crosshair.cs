using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : CanvasUI {
    [SerializeField]
    private RectTransform cornOfFireIndicator;

    public float extent = 10;

    private Image[] images;
    private Coroutine hitMarkRoutine;

    private void Awake() {
        this.images = GetComponentsInChildren<Image>(true);
    }

    private void Update() {
        if (this.cornOfFireIndicator == null) {
            return;
        }
        float size = this.cornOfFireIndicator.sizeDelta.x;
        if (size == this.extent) {
            return;
        }
        if (Mathf.Abs(size - this.extent) < 0.1) {
            this.cornOfFireIndicator.sizeDelta = new Vector2(this.extent, this.extent);
            return;
        }
        this.cornOfFireIndicator.sizeDelta = Vector2.Lerp(this.cornOfFireIndicator.sizeDelta, new Vector2(this.extent, this.extent), Time.deltaTime * 20f);
    }

    public void ShowHitMark(float duration = 1f) {
        if (this.hitMarkRoutine != null) {
            StopCoroutine(this.hitMarkRoutine);
        }
        this.hitMarkRoutine = StartCoroutine(HitMarkRoutine(duration));
    }

    private IEnumerator HitMarkRoutine(float duration = 1f) {
        SetColor(Color.red);
        yield return new WaitForSeconds(duration);
        SetColor(Color.white);
    }

    private void SetColor(Color color) {
        foreach (Image i in this.images) {
            i.color = color;
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : CanvasUI {
    [SerializeField]
    private RectTransform coneOfFireIndicator = default;

    public float ConeOfFireSize { get; private set; } = 10f;

    private Image[] images;
    private Coroutine hitMarkRoutine;

    private void Awake() {
        this.images = GetComponentsInChildren<Image>(true);
    }

    private void Update() {
        if (this.coneOfFireIndicator == null) {
            return;
        }
        float size = this.coneOfFireIndicator.sizeDelta.x;
        if (size == this.ConeOfFireSize) {
            return;
        }
        if (Mathf.Abs(size - this.ConeOfFireSize) < 0.1) {
            this.coneOfFireIndicator.sizeDelta = new Vector2(this.ConeOfFireSize, this.ConeOfFireSize);
            return;
        }
        this.coneOfFireIndicator.sizeDelta = Vector2.Lerp(this.coneOfFireIndicator.sizeDelta, new Vector2(this.ConeOfFireSize, this.ConeOfFireSize), Time.deltaTime * 20f);
    }

    public void UpdateConeOfFire(BulletSpread spread) {
        this.ConeOfFireSize = spread.RadiusInScreen * 2f;
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

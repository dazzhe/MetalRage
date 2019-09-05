using UnityEngine;
using UnityEngine.UI;

public class Gunsight : MonoBehaviour {
    [SerializeField]
    private RectTransform controllableSection;

    public float extent = 10;

    private Image[] images;

    private void Start() {
        this.images = GetComponentsInChildren<Image>(true);
    }

    private void Update() {
        if (this.controllableSection == null) {
            return;
        }

        float size = this.controllableSection.sizeDelta.x;
        if (size == this.extent) {
            return;
        }

        if (Mathf.Abs(size - this.extent) < 0.1) {
            this.controllableSection.sizeDelta = new Vector2(this.extent, this.extent);
            return;
        }
        this.controllableSection.sizeDelta = Vector2.Lerp(this.controllableSection.sizeDelta, new Vector2(this.extent, this.extent), Time.deltaTime * 20f);
    }

    public void SetColor(Color color) {
        foreach (Image i in this.images) {
            i.color = color;
        }
    }

    public void Hide() {
        foreach (Transform child in this.transform) {
            child.gameObject.SetActive(false);
        }
    }

    public void Show() {
        foreach (Transform child in this.transform) {
            child.gameObject.SetActive(true);
        }
    }
}

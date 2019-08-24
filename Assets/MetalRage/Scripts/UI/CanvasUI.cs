using UnityEngine;

public class CanvasUI : MonoBehaviour {
    protected Canvas canvas;

    protected Canvas Canvas {
        get {
            if (this.canvas == null) {
                this.canvas = GetComponent<Canvas>();
            }
            return this.canvas;
        }
    }

    public bool IsVisible {
        get => this.Canvas.enabled;
        set => this.Canvas.enabled = value;
    }

    public void Show() {
        this.IsVisible = true;
    }

    public void Hide() {
        this.IsVisible = false;
    }

    public void Toggle() {
        this.IsVisible = !this.IsVisible;
    }
}

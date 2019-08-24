using UnityEngine;

public class MenuUI : CanvasUI {
    // ActiveWindowLevel is the hierarchy of the showing window.
    // 0 = no window. 1 = menu. 2 = other windows.
    public int ActiveWindowLevel { get; set; } = 0;

    private void Update() {
        if (Input.GetButtonDown("Menu") && this.ActiveWindowLevel <= 1) {
            ToggleCanvas();
        }
    }

    public void ToggleCanvas() {
        Toggle();
        if (this.IsVisible) {
            this.ActiveWindowLevel = 1;
            UIManager.Instance.ShowCursor();
        } else if (this.ActiveWindowLevel == 1) {
            this.ActiveWindowLevel = 0;
            UIManager.Instance.HideCursor();
        }
    }
}

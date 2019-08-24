using UnityEngine;

public class UIManager : SingletonBehaviour<UIManager> {
    [SerializeField]
    private MenuUI menuUI;
    [SerializeField]
    private StatusUI statusUI;
    [SerializeField]
    private ScoreBoardUI scoreBoardUI;

    private bool isCursorVisible = true;

    public MenuUI MenuUI {
        get {
            if (this.menuUI == null) {
                this.menuUI = this.transform.Find("MenuUI").GetComponent<MenuUI>();
            }
            return this.menuUI;
        }
        private set => this.menuUI = value;
    }

    public StatusUI StatusUI {
        get {
            if (this.statusUI == null) {
                this.statusUI = this.transform.Find("StatusUI").GetComponent<StatusUI>();
            }
            return this.statusUI;
        }
        private set => this.statusUI = value;
    }

    public ScoreBoardUI ScoreBoardUI {
        get {
            if (this.scoreBoardUI == null) {
                var obj = this.transform.Find("ScoreBoardUI");
                this.scoreBoardUI = obj?.GetComponent<ScoreBoardUI>();
            }
            return this.scoreBoardUI;
        }
        private set => this.scoreBoardUI = value;
    }

    public void ShowCursor() {
        this.isCursorVisible = true;
    }

    public void HideCursor() {
        this.isCursorVisible = false;
    }

    private void Start() {
        EnableDdol = false;
    }

    private void Update() {
        if (this.isCursorVisible) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

using UnityEngine;

public class UnitOption : MonoBehaviour {
    public string unit;
    private GameManager gm;
    private static UnitOption instance;

    private void Awake() {
        this.gm = GetComponent<GameManager>();
        instance = this;
    }

    public static void UnitSelect() {
        instance.enabled = true;
        UIManager.Instance.ShowCursor();
    }

    private void OnGUI() {
        if (GUI.Button(new Rect(20, 40, 80, 20), "Vanguard")) {
            this.unit = "Vanguard";
            this.gm.SpawnLocalPlayer(this.unit);
            UIManager.Instance.HideCursor();
            UIManager.Instance.StatusUI.Show();
        }
        if (GUI.Button(new Rect(20, 70, 80, 20), "Dual")) {
            this.unit = "Dual";
            this.gm.SpawnLocalPlayer(this.unit);
            UIManager.Instance.HideCursor();
            UIManager.Instance.StatusUI.Show();
        }
        if (GUI.Button(new Rect(20, 100, 80, 20), "Blitz")) {
            this.unit = "Blitz";
            this.gm.SpawnLocalPlayer(this.unit);
            UIManager.Instance.HideCursor();
            UIManager.Instance.StatusUI.Show();
        }
        if (GUI.Button(new Rect(20, 130, 80, 20), "Velox")) {
            this.unit = "Velox";
            this.gm.SpawnLocalPlayer(this.unit);
            UIManager.Instance.HideCursor();
            UIManager.Instance.StatusUI.Show();
        }
        if (GUI.Button(new Rect(20, 160, 80, 20), "Vincere")) {
            this.unit = "Vincere";
            this.gm.SpawnLocalPlayer(this.unit);
            UIManager.Instance.HideCursor();
            UIManager.Instance.StatusUI.Show();
        }
    }
}

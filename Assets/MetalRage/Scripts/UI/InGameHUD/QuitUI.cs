using UnityEngine;

public class QuitUI : MonoBehaviour {
    public void OnClickOK() {
        Application.Quit();
    }

    public void OnClickedCancel() {
        this.gameObject.SetActive(false);
        UIManager.Instance.MenuUI.ActiveWindowLevel = 1;
    }

    public void ShowWindow() {
        this.gameObject.SetActive(true);
        UIManager.Instance.MenuUI.ActiveWindowLevel = 2;
    }
}

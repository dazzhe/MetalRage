using UnityEngine;

public class QuitUI : MonoBehaviour {
    public void OnClickOK() {
        Application.Quit();
    }

    public void OnClickedCancel() {
        this.gameObject.SetActive(false);
    }

    public void ShowWindow() {
        this.gameObject.SetActive(true);
    }
}

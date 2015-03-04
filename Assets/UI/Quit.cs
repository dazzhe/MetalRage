using UnityEngine;
using System.Collections;

public class Quit : MonoBehaviour {
	[SerializeField] Canvas canvas;

	public void OnClickedOK()
	{
		Application.Quit();
	}

	public void OnClickedCancel()
	{
		canvas.enabled = false;
		Menu.activeWindowLevel = 1;
	}

	public void ShowWindow()
	{
		canvas.enabled = true;
		Menu.activeWindowLevel = 2;
	}
}

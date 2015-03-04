using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour {
	[SerializeField] Canvas canvas;

	//activeWindowLevel is the hierarchy of the showing window
	//0 = no window. 1 = menu. 2 = other windows
	public static int activeWindowLevel = 0;

	void Start ()
	{

	}
	
	void Update ()
	{
		if (Input.GetButtonDown("Menu") && activeWindowLevel <= 1) {
			ToggleCanvas();
		}
	}

	public void ToggleCanvas()
	{
		canvas.enabled = !canvas.enabled;
		
		if (canvas.enabled) {
			activeWindowLevel = 1;
			ShowCursor();
		} else if (activeWindowLevel == 1) {
			activeWindowLevel = 0;
		}
	}

	void ShowCursor()
	{
		Screen.lockCursor = false;
		Screen.showCursor = true;
	}
}

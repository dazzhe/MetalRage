using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Sight : MonoBehaviour {
	private RectTransform controllableSection;
	private Image[] images;

	void Start () {
		if (transform.Find("ControllableSection"))
			controllableSection = transform.Find("ControllableSection").GetComponent<RectTransform>();
		images = GetComponentsInChildren<Image>();
	}
	

	public void SetColor(Color color){
		foreach (Image i in images)
			i.color = color;
	}

	public void SetArea(float size){
		controllableSection.sizeDelta = new Vector2(size, size);
	}

	public void HideSight(){
		foreach (Transform child in transform)
			child.gameObject.SetActive(false);
	}

	public void ShowSight(){
		foreach (Transform child in transform)
			child.gameObject.SetActive(true);
	}
}

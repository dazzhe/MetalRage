using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Sight : MonoBehaviour {
	private RectTransform controllableSection;
	private Image[] images;
	public float extent = 10;

	void Start () {
		if (transform.Find("ControllableSection"))
			controllableSection = transform.Find("ControllableSection").GetComponent<RectTransform>();
		images = GetComponentsInChildren<Image>(true);
	}
	
	void Update(){
		float size = controllableSection.sizeDelta.x;
		if (size == extent) return;
		if (Mathf.Abs(size - extent) < 0.1){
			controllableSection.sizeDelta = new Vector2(extent, extent);
			return;
		}
		controllableSection.sizeDelta = Vector2.Lerp (controllableSection.sizeDelta, new Vector2(extent, extent), Time.deltaTime * 20f);
	}

	public void SetColor(Color color){
		foreach (Image i in images)
			i.color = color;
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

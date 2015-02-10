using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NormalDisplay : MonoBehaviour {
	public Text HPtext;
	public static Text NOLtext;
	Slider boostgauge;
	public Slider HPBar;

	void Start () {
		boostgauge = transform.FindChild ("BoostGauge").GetComponent<Slider>();
		HPBar = transform.FindChild ("HPBar").GetComponent<Slider>();
		HPtext = transform.FindChild ("HPText").GetComponent<Text>();
		NOLtext = transform.FindChild ("NOLText").GetComponent<Text>();
	}

	public void SetBoostGauge(int boost){
		boostgauge.value = boost / 100f;
	}
}
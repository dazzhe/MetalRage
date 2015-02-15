using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NormalDisplay : MonoBehaviour {
	private static Text HPValue;
	private static Slider HPBar;
	private static Text magazineValue;
	private static Text ammoValue;
	private static Slider boostGauge;
	
	void Start ()
	{
		boostGauge = transform.FindChild ("BoostGauge").GetComponent<Slider>();
		HPValue = transform.FindChild ("Status/HPValue").GetComponent<Text>();
		HPBar = transform.FindChild ("Status/HPBar").GetComponent<Slider>();
		magazineValue = transform.FindChild ("Ammunition/Magazine").GetComponent<Text>();
		ammoValue = transform.FindChild ("Ammunition/Ammo").GetComponent<Text>();
	}

	public static void SetBoostGauge(int boost)
	{
		boostGauge.value = boost / 100f;
	}

	public static void SetHP(int HP, int maxHP)
	{
		HPValue.text = HP.ToString("D3");
		HPBar.value = (float)HP / (float)maxHP;
	}

	public static void SetMagazine(int magazine)
	{
		magazineValue.text = magazine.ToString("D3");
	}

	public static void SetAmmo(int ammo)
	{
		ammoValue.text = ammo.ToString("D4");
	}
}
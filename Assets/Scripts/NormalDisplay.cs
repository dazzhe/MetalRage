using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NormalDisplay : MonoBehaviour {
	public Text HPtext;
	Text killcounter;
	Text deathcounter;
	Slider boostgauge;
	public Slider HPBar;
	static Image reticleC;
	static Image reticleD;
	static Image reticleU;
	static Image reticleL;
	static Image reticleR;

	int kill;
	int death;

	// Use this for initialization
	void Start () {
		killcounter = transform.FindChild ("KillCounter").GetComponent<Text>();
		deathcounter = transform.FindChild ("DeathCounter").GetComponent<Text>();
		boostgauge = transform.FindChild ("BoostGauge").GetComponent<Slider>();
		HPBar = transform.FindChild ("HPBar").GetComponent<Slider>();
		HPtext = transform.FindChild ("HPText").GetComponent<Text>();
		reticleC = transform.FindChild ("reticle").FindChild ("center").GetComponent<Image>();
		reticleD = transform.FindChild ("reticle").FindChild ("down").GetComponent<Image>();
		reticleU = transform.FindChild ("reticle").FindChild ("up").GetComponent<Image>();
		reticleL = transform.FindChild ("reticle").FindChild ("left").GetComponent<Image>();
		reticleR = transform.FindChild ("reticle").FindChild ("right").GetComponent<Image>();
		reticleC.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
		kill = 0;
		death = 0;
		killcounter.text = "Kill: " + kill;
		deathcounter.text = "Death: " + death;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void KillCount(){
		this.kill++;
		killcounter.text = "Kill: " + kill;
	}

	public void DeathCount(){
		this.death++;
		deathcounter.text = "Death: " + death;
	}

	public void SetBoostGauge(int boost){
		boostgauge.value = boost / 100f;
	}

	public static void SetReticle(float dispersion){
		reticleU.transform.position = new Vector3(Screen.width/2, Screen.height/2 + dispersion ,0);
		reticleD.transform.position = new Vector3(Screen.width/2, Screen.height/2 - dispersion, 0);
		reticleL.transform.position = new Vector3(Screen.width/2 - dispersion, Screen.height/2, 0);
		reticleR.transform.position = new Vector3(Screen.width/2 + dispersion, Screen.height/2, 0);
	}

	public static void RedReticle(){
		reticleC.color = Color.red;
		reticleD.color = Color.red;
		reticleU.color = Color.red;
		reticleL.color = Color.red;
		reticleR.color = Color.red;
	}

	public static void WhiteReticle(){
		reticleC.color = Color.white;
		reticleD.color = Color.white;
		reticleU.color = Color.white;
		reticleL.color = Color.white;
		reticleR.color = Color.white;
	}
}
using UnityEngine;
using System.Collections;

public class UnitOption : MonoBehaviour {
	public string unit;
	GameManager gm;
	// Use this for initialization
	void Start () {
		gm = GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		if(GUI.Button(new Rect(20,40,80,20), "Vanguard")) {
			unit = "Vanguard";
			gm.Spawn(unit);
		}
	
		//if(GUI.Button(new Rect(20,70,80,20), "Blitz")) {
		//	unit = "Blitz";
		//}

		if(GUI.Button(new Rect(20,100,80,20), "Dual")) {
			unit = "Dual";
			gm.Spawn(unit);
		}
	}
}

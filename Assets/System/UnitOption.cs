using UnityEngine;
using System.Collections;

public class UnitOption : MonoBehaviour {
	public string unit;
	GameManager gm;
	static UnitOption uo;

	void Awake () {
		gm = GetComponent<GameManager>();
		uo = this;
	}

	public static void UnitSelect(){
		uo.enabled = true;
		Screen.lockCursor = false;
		Screen.showCursor = true;
	}

	void OnGUI(){
		if(GUI.Button(new Rect(20,40,80,20), "Vanguard")) {
			unit = "Vanguard";
			gm.Spawn(unit);
		}
	
		if(GUI.Button(new Rect(20,70,80,20), "Dual")) {
			unit = "Dual";
			gm.Spawn (unit);
		}

		if(GUI.Button(new Rect(20,100,80,20), "Blitz")) {
			unit = "Blitz";
			gm.Spawn(unit);
		}

		if(GUI.Button(new Rect(20,130,80,20), "Velox")) {
			unit = "Velox";
			gm.Spawn(unit);
		}
	}
}

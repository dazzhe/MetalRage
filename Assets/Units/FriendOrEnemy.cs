using UnityEngine;
using System.Collections;

public class FriendOrEnemy : MonoBehaviour {
	private PhotonView myPV;
	public int team = 5;
	public string playerName;
	private Vector2 nameTagPos;
	GUIStyle style;

	void Start () {
		myPV = GetComponent<PhotonView>();
		style = new GUIStyle();
		if (myPV.isMine)
			myPV.RPC("Sync",PhotonTargets.OthersBuffered,team,playerName);
		StartCoroutine(WaitAndProceed ());
	}

	[RPC]
	void Sync(int t, string name){
		playerName = name;
		team = t;
	}

	//Wait until we select team and recieve team of this unit
	IEnumerator WaitAndProceed(){
		yield return new WaitForSeconds(0.5f);//Somehow sometimes team becomes 0 only at first so...
		while (GameManager.myTeam == 5 || team == 5)
			yield return new WaitForSeconds(0.5f);
		if (!myPV.isMine){
			if (GameManager.myTeam == team){
				Debug.Log ("myteam="+GameManager.myTeam.ToString());
				Debug.Log ("team="+team.ToString());
				this.gameObject.SetLayerRecursively(9);//Teammate
				style.fontSize = 20;
				style.normal.textColor = Color.green;
			} else {
				this.gameObject.SetLayerRecursively(10);//Enemy
				this.enabled = false;
			}
		} else
			this.enabled = false;
	}

	void OnGUI() {
		nameTagPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		//Display nametag if the unit is ahead
		if (Vector3.Dot(gameObject.transform.position - Camera.main.transform.position,
		                Camera.main.transform.TransformDirection(Vector3.forward)) > 0)
			GUI.Label(new Rect(nameTagPos.x,
			                   Screen.height - nameTagPos.y,
			                   100,
		        	           50),
		    	      playerName, style);
	}
}
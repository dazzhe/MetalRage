using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class InRoomChat : Photon.MonoBehaviour 
{
    private Rect GuiRect = new Rect(Screen.width - 250, Screen.height - 300, 250, 300);
    public bool IsVisible = true;
	public bool canInput = false;
    public bool AlignBottom = false;
    public List<string> messages = new List<string>();
    private string inputLine = "";
    private Vector2 scrollPos = Vector2.zero;

    public static readonly string ChatRPC = "Chat";

    public void Start(){
		if (string.IsNullOrEmpty(PhotonNetwork.playerName)) {   
			PhotonNetwork.playerName = "Guest" + Random.Range(1, 999);     
		}  
        if (AlignBottom) {
            GuiRect.y = Screen.height - GuiRect.height;
        }
    }

	void Update(){
		if (Input.GetKeyDown (KeyCode.T) && !canInput)
			StartCoroutine("StartChat");
	}

	IEnumerator StartChat(){
		yield return new WaitForSeconds(0.1f);
		canInput = true;
	}

    public void OnGUI(){
        if (!IsVisible || PhotonNetwork.connectionStateDetailed != PeerState.Joined){
            return;
        }
        
        if (canInput && Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)){
            if (!string.IsNullOrEmpty(inputLine)) {
                this.photonView.RPC("Chat", PhotonTargets.All, inputLine);
                this.inputLine = "";
                //GUI.FocusControl("");
				canInput = false;
                return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
            }
            else {
                //GUI.FocusControl("ChatInput");
				//Screen.lockCursor = false;
				//Screen.showCursor = true;
				canInput = false;
				return;
			}
        }

		GUI.SetNextControlName("");
		GUILayout.BeginArea(this.GuiRect);

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.FlexibleSpace();
        for (int i = messages.Count - 1; i >= 0; i--){
            GUILayout.Label(messages[i]);
        }
        GUILayout.EndScrollView();

		if (canInput){
	        GUILayout.BeginHorizontal();
	        GUI.SetNextControlName("ChatInput");
   		    inputLine = GUILayout.TextField(inputLine);
   		    if (GUILayout.Button("Send", GUILayout.ExpandWidth(false))) {
   	      		this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
           		this.inputLine = "";
           		GUI.FocusControl("");
        	}
        	GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		if (GUI.GetNameOfFocusedControl() == "")
			GUI.FocusControl("ChatInput");
	}

    [RPC]
    public void Chat(string newLine, PhotonMessageInfo mi) {
        string senderName = "anonymous";

        if (mi != null && mi.sender != null){
            if (!string.IsNullOrEmpty(mi.sender.name)){
                senderName = mi.sender.name;
            }
            else {
                senderName = "player " + mi.sender.ID;
            }
        }

        this.messages.Add(senderName +": " + newLine);
    }

    public void AddLine(string newLine) {
        this.messages.Add(newLine);
    }
}

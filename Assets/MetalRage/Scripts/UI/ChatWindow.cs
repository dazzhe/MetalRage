using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ChatWindow : Photon.MonoBehaviour {
    public Rect GuiRect = new Rect(0, 0, 250, 300);
    public bool IsVisible = true;
    public bool AlignBottom = false;
    public List<string> messages = new List<string>();
    private string inputLine = "";
    private Vector2 scrollPos = Vector2.zero;

    public static readonly string ChatRPC = "Chat";

    public void Start() {
        if (this.AlignBottom) {
            this.GuiRect.y = Screen.height - this.GuiRect.height;
        }
    }

    public void OnGUI() {
        if (!this.IsVisible || !PhotonNetwork.inRoom) {
            return;
        }

        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)) {
            if (!string.IsNullOrEmpty(this.inputLine)) {
                this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
                this.inputLine = "";
                GUI.FocusControl("");
                return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
            } else {
                GUI.FocusControl("ChatInput");
            }
        }

        GUI.SetNextControlName("");
        GUILayout.BeginArea(this.GuiRect);

        this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);
        GUILayout.FlexibleSpace();
        for (int i = this.messages.Count - 1; i >= 0; i--) {
            GUILayout.Label(this.messages[i]);
        }
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("ChatInput");
        this.inputLine = GUILayout.TextField(this.inputLine);
        if (GUILayout.Button("Send", GUILayout.ExpandWidth(false))) {
            this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
            this.inputLine = "";
            GUI.FocusControl("");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    [PunRPC]
    public void Chat(string newLine, PhotonMessageInfo mi) {
        string senderName = "anonymous";

        if (mi.sender != null) {
            if (!string.IsNullOrEmpty(mi.sender.NickName)) {
                senderName = mi.sender.NickName;
            } else {
                senderName = "player " + mi.sender.ID;
            }
        }

        this.messages.Add(senderName + ": " + newLine);
    }

    public void AddLine(string newLine) {
        this.messages.Add(newLine);
    }
}
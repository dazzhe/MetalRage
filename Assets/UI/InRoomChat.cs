using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class InRoomChat : Photon.MonoBehaviour 
{
	[SerializeField] InputField inputField;
	[SerializeField] GameObject chatList;

    public static readonly string ChatRPC = "Chat";

    public void Start()
	{
		if (string.IsNullOrEmpty(PhotonNetwork.playerName)) {
			PhotonNetwork.playerName = "Guest" + Random.Range(1, 999);
		}
    }

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.T) && Menu.activeWindowLevel == 0){
			StartCoroutine("StartChat");
		}
		if (inputField.isFocused){
			Menu.activeWindowLevel = 3;
		} else if (Menu.activeWindowLevel == 3) {
			Menu.activeWindowLevel = 0;
		}
	}

	IEnumerator StartChat()
	{
		yield return new WaitForSeconds(0.1f);
		inputField.gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
		inputField.OnPointerClick(new PointerEventData(EventSystem.current));
	}

	public void SendChat()
	{
		if (inputField.text != ""){
			this.photonView.RPC("SendChatRPC", PhotonTargets.All, inputField.text);
			inputField.text = "";
		}
		inputField.gameObject.SetActive(false);
	}

	[RPC]
	void SendChatRPC(string text, PhotonMessageInfo mi)
	{
		string senderName = "anonymous";
		
		if (mi != null && mi.sender != null){
			if (!string.IsNullOrEmpty(mi.sender.name)){
				senderName = mi.sender.name;
			}
			else {
				senderName = "player " + mi.sender.ID;
			}
		}

		GameObject _chatText = (GameObject)Instantiate(Resources.Load ("ChatText"));
		_chatText.transform.parent = chatList.transform;
		_chatText.transform.localScale = new Vector3(1f, 1f, 1f);
		Text chatText = _chatText.GetComponent<Text>();
		chatText.text = senderName + ": " + text;
	}
}

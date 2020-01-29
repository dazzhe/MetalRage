//using System.Collections;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//[RequireComponent(typeof(PhotonView))]
//public class ChatUI : Photon.MonoBehaviour {
//    [SerializeField]
//    private InputField inputField = default;
//    [SerializeField]
//    private GameObject chatList = default;
//    [SerializeField]
//    private GameObject chatTextPrefab = default;

//    public void Start() {
//        if (string.IsNullOrEmpty(PhotonNetwork.playerName)) {
//            PhotonNetwork.playerName = "Guest" + Random.Range(1, 999);
//        }
//    }

//    private void Update() {
//        var menu = UIManager.Instance.MenuUI;
//        if (Input.GetKeyDown(KeyCode.T) && menu.ActiveWindowLevel == 0) {
//            StartCoroutine(StartChat());
//        }
//        if (this.inputField.isFocused) {
//            menu.ActiveWindowLevel = 3;
//        } else if (menu.ActiveWindowLevel == 3) {
//            menu.ActiveWindowLevel = 0;
//        }
//    }

//    private IEnumerator StartChat() {
//        yield return new WaitForSeconds(0.1f);
//        this.inputField.gameObject.SetActive(true);
//        EventSystem.current.SetSelectedGameObject(this.inputField.gameObject, null);
//        this.inputField.OnPointerClick(new PointerEventData(EventSystem.current));
//    }

//    public void SendChat() {
//        if (this.inputField.text != "") {
//            this.photonView.RPC("SendChatRPC", PhotonTargets.All, this.inputField.text);
//            this.inputField.text = "";
//        }
//        this.inputField.gameObject.SetActive(false);
//    }

//    [PunRPC]
//    private void SendChatRPC(string text, PhotonMessageInfo mi) {
//        string senderName = "anonymous";
//        if (mi.sender != null) {
//            if (!string.IsNullOrEmpty(mi.sender.NickName)) {
//                senderName = mi.sender.NickName;
//            } else {
//                senderName = "player " + mi.sender.ID;
//            }
//        }
//        GameObject chatTextObj = Instantiate(this.chatTextPrefab);
//        chatTextObj.transform.parent = this.chatList.transform;
//        chatTextObj.transform.localScale = new Vector3(1f, 1f, 1f);
//        Text chatText = chatTextObj.GetComponent<Text>();
//        chatText.text = senderName + ": " + text;
//    }
//}
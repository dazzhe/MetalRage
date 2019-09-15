using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardUI : CanvasUI {
    private Text scoreR;
    private Text scoreB;
    [SerializeField]
    private GameObject redScoreList;
    [SerializeField]
    private GameObject blueScoreList;

    private bool isAlwaysVisible = false;

    public static PlayerScoreEntry myEntry;

    public GameObject RedScoreList { get => this.redScoreList; set => this.redScoreList = value; }
    public GameObject BlueScoreList { get => this.blueScoreList; set => this.blueScoreList = value; }

    private void Awake() {
        this.scoreR = this.transform.Find("Panel/REDScore/Text").GetComponent<Text>();
        this.scoreB = this.transform.Find("Panel/BLUEScore/Text").GetComponent<Text>();
    }

    /// <summary>
    /// This will be called from <see cref="Networking.OnJoinedRoom()"></see>.
    /// </summary>
    public void SelectTeam() {
        this.isAlwaysVisible = true;
    }

    private void Update() {
        this.scoreR.text = GameManager.Instance.GetTeam(TeamColor.Red).Score.ToString("D3");
        this.scoreB.text = GameManager.Instance.GetTeam(TeamColor.Blue).Score.ToString("D3");
        if (this.isAlwaysVisible) {
            if (!this.IsVisible) {
                Show();
            }
            return;
        }
        if (Input.GetButton("ScoreBoard") && !this.IsVisible) {
            Show();
        } else if (!Input.GetButton("ScoreBoard") && this.IsVisible) {
            Hide();
        }
    }

    // This will be called when the buttons on scoreboard are clicked.
    public void AddRedPlayer() {
        AddPlayer(TeamColor.Red);
    }

    public void AddBluePlayer() {
        AddPlayer(TeamColor.Blue);
    }

    private void AddPlayer(TeamColor team) {
        GameManager.Instance.PlayerTeam = team;
        PhotonNetwork.player.CustomProperties["Team"] = team;
        PhotonNetwork.player.SetCustomProperties(PhotonNetwork.player.CustomProperties);
        GameObject myEntry = PhotonNetwork.Instantiate("PlayerScoreEntry",
            Vector3.zero, Quaternion.identity, 0);
        ScoreboardUI.myEntry = myEntry.GetComponent<PlayerScoreEntry>();
        ScoreboardUI.myEntry.team = team;
        UnitOption.UnitSelect();
        this.isAlwaysVisible = false;
    }
}
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
    public static bool startAdding = false;
    public static int[] score = { 0, 0 };

    public GameObject RedScoreList { get => this.redScoreList; set => this.redScoreList = value; }
    public GameObject BlueScoreList { get => this.blueScoreList; set => this.blueScoreList = value; }

    private void Awake() {
        this.scoreR = this.transform.Find("Panel/REDScore/Text").GetComponent<Text>();
        this.scoreB = this.transform.Find("Panel/BLUEScore/Text").GetComponent<Text>();
        StartCoroutine(ScoreUpdateLoop());
    }

    /// <summary>
    /// This will be called from <see cref="Networking.OnJoinedRoom()"></see>.
    /// </summary>
    public void SelectTeam() {
        this.isAlwaysVisible = true;
    }

    private void Update() {
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
    public void AddPlayer(int team) {
        GameManager.PlayerTeam = team;
        GameObject myEntry
            = PhotonNetwork.Instantiate("PlayerScoreEntry",
                                        Vector3.zero,
                                        Quaternion.identity, 0) as GameObject;
        ScoreboardUI.myEntry = myEntry.GetComponent<PlayerScoreEntry>();
        ScoreboardUI.myEntry.team = team;
        UnitOption.UnitSelect();
        this.isAlwaysVisible = false;
    }

    private IEnumerator ScoreUpdateLoop() {
        while (true) {
            if (startAdding) {
                score[0] = 0;
                score[1] = 0;
                this.transform.BroadcastMessage("AddScore");
                this.scoreR.text = score[0].ToString("D3");
                this.scoreB.text = score[1].ToString("D3");
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}
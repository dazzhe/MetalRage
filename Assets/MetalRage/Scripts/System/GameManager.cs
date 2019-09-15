using System.Linq;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager> {
    [SerializeField]
    private Team blueTeam = new Team(TeamColor.Blue, default);
    [SerializeField]
    private Team redTeam = new Team(TeamColor.Red, default);

    public TeamColor PlayerTeam { get; set; } = TeamColor.None;

    public Team GetTeam(TeamColor teamColor) {
        switch (teamColor) {
            case TeamColor.Blue:
                return this.blueTeam;
            case TeamColor.Red:
                return this.redTeam;
            default:
                return null;
        }
    }

    public void SpawnLocalPlayer(string unit) {
        Vector3 spawnPosition = GetTeam(this.PlayerTeam).SpawnArea.RandomPosition();
        GameObject playerObj = PhotonNetwork.Instantiate(unit, spawnPosition, Quaternion.identity, 0);
        GetComponent<UnitOption>().enabled = false;
        playerObj.SetLayerRecursively(8);
        playerObj.GetComponent<UnitController>().enabled = true;
        playerObj.GetComponent<UnitMotor>().enabled = true;
        playerObj.GetComponent<WeaponControl>().enabled = true;
        playerObj.GetComponent<FollowingCamera>().enabled = true;
    }

    public void KillLocalPlayer() {
        ScoreboardUI.myEntry.IncrementDeath();
        UnitOption.UnitSelect();
    }

    public void QuitApplication() {
        Application.Quit();
    }
}

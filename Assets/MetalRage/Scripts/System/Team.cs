using UnityEngine;

[System.Serializable]
public class Team {
    private readonly string scoreKey;

    [SerializeField]
    private SpawnArea spawnArea;

    public TeamColor Color { get; set; }

    public int Score {
        get {
            if (PhotonNetwork.room == null) {
                return 0;
            }
            if (PhotonNetwork.room.CustomProperties[this.scoreKey] == null) {
                this.Score = 0;
                return 0;
            }
            return (int)PhotonNetwork.room.CustomProperties[this.scoreKey];
        }
        set {
            if (!PhotonNetwork.player.IsMasterClient) {
                Debug.LogError("Don't set room property beside from master client.");
                return;
            }
            PhotonNetwork.room.CustomProperties[this.scoreKey] = value;
            PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.CustomProperties);
        }
    }

    public SpawnArea SpawnArea { get => this.spawnArea; set => this.spawnArea = value; }

    public Team(TeamColor teamColor, SpawnArea spawnArea) {
        this.Color = teamColor;
        this.SpawnArea = spawnArea;
        switch (teamColor) {
            case TeamColor.Blue:
                this.scoreKey = "blueTeamScore";
                break;
            case TeamColor.Red:
                this.scoreKey = "redTeamScore";
                break;
        }
    }
}
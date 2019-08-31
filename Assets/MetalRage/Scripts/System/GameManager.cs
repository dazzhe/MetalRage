using UnityEngine;

public class GameManager : MonoBehaviour {
    //respawnX[0]<-RespawnRangeX of RedTeam
    //respawnX[1]<-RespawnRangeX of BlueTeam...
    private float[,] respawnX = new float[2, 2] { { -92, -40 }, { -43, 7 } };
    private float[] respawnY = new float[2] { 6, 9 };
    private float[,] respawnZ = new float[2, 2] { { -126, -114 }, { 182, 190 } };
    public static int PlayerTeam { get; set; } = 5;   //<-if RedTeam then 0 else 1

    private PhotonView photonView;
    private GameObject explosion;

    private void Awake() {
        this.photonView = GetComponent<PhotonView>();
    }

    public void Spawn(string unit) {
        Vector3 spawnPos = new Vector3(Random.Range(this.respawnX[PlayerTeam, 0], this.respawnX[PlayerTeam, 1]),
                                       this.respawnY[PlayerTeam],
                                       Random.Range(this.respawnZ[PlayerTeam, 0], this.respawnZ[PlayerTeam, 1]));
        GameObject player = PhotonNetwork.Instantiate(unit, spawnPos, Quaternion.identity, 0);
        GetComponent<UnitOption>().enabled = false;
        player.SetLayerRecursively(8);
        player.GetComponent<UnitController>().enabled = true;
        player.GetComponent<UnitMotor>().enabled = true;
        player.GetComponent<WeaponControl>().enabled = true;
        player.GetComponent<FollowingCamera>().enabled = true;
    }

    public void Die(Vector3 pos, GameObject go) {
        PhotonNetwork.Destroy(go);
        UnitOption.UnitSelect();
    }

    public void QuitApplication() {
        Application.Quit();
    }
}

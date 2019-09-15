using UnityEngine;

[System.Serializable]
public class SpawnArea {
    [SerializeField]
    private BoxCollider collider;

    public Vector3 Center
        => this.collider.transform.position + this.collider.transform.rotation * this.collider.center;
    public Vector3 Size
        => this.collider.transform.rotation * this.collider.size;

    public SpawnArea() { }

    public Vector3 RandomPosition() {
        var min = this.Center - this.Size / 2f;
        var max = this.Center + this.Size / 2f;
        var x = Random.Range(min.x, max.x);
        var y = Random.Range(min.y, max.y);
        var z = Random.Range(min.z, max.z);
        return new Vector3(x, y, z);
    }
}
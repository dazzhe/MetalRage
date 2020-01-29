using UnityEngine;

public class TimedSelfDestructor : MonoBehaviour {
    [SerializeField]
    private float lifeTime = 1f;

    public float LifeTime { get => this.lifeTime; set => this.lifeTime = value; }

    private void Start() {
        Destroy(this.gameObject, this.LifeTime);
    }
}

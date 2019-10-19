using UnityEngine;

public class Hit : MonoBehaviour {
    private Status status;
    public float defence = 1f;

    private void Start() {
        this.status = GetComponentInParent<Status>();
    }

    public void TakeDamage(int damage, string attackerName) {
        this.status.ReduceHP(Mathf.FloorToInt(damage / this.defence), attackerName);
    }
}

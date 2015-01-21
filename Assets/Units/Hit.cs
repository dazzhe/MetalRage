using UnityEngine;
using System.Collections;

public class Hit : MonoBehaviour {
	Status stat;
	public float defence = 1f;

	void Start () {
		stat = GetComponentInParent<Status>();
	}

	public void TakeDamage(int damage, GameObject attackedPlayer){
		stat.ReduceHP(Mathf.FloorToInt(damage * defence), attackedPlayer);
	}
}
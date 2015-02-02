using UnityEngine;
using System.Collections;

public class Hit : MonoBehaviour {
	Status stat;
	public float defence = 1f;

	void Start () {
		stat = GetComponentInParent<Status>();
	}
	
	public void TakeDamage(int damage, string attacker){
		if (damage > 0)
			stat.ReduceHP(Mathf.FloorToInt(damage * defence), attacker);
		else
			stat.ReduceHP(Mathf.FloorToInt(damage), attacker);	//not to reduce repair
	}
}
using UnityEngine;
using System.Collections;

public abstract class WeaponRay : Weapon {
	protected GameObject targetObject;
	protected Vector3 targetPos;
	protected Ray ray;
	protected RaycastHit hit;
	protected RaycastHit hinderinghit;
	
	protected IEnumerator ShotControl(){
		if (wcontrol.inputShot1 && load > 0 && !cooldown && canShot && !isReloading){
			Shot();
			myPV.RPC("MakeShots",PhotonTargets.All);
			cooldown = true;
			yield return new WaitForSeconds(interval);
			cooldown = false;
		}
	}
	
	protected void Shot(){
		float r = Random.value * mindispersion * wcontrol.desiredDispersion;
		float theta = Random.value * 2 * Mathf.PI;
		Vector3 shotpoint = wcontrol.center + new Vector3(r * Mathf.Cos (theta), r * Mathf.Sin (theta), 0);
		ray = Camera.main.ScreenPointToRay(shotpoint);
		if (Physics.Raycast(ray, out hit, maxrange, wcontrol.mask)) {
			targetPos = hit.point;
			targetObject = hit.collider.gameObject;
			
			//狙っているオブジェクトと機体の間に障害物があった場合.
			//障害物で弾がヒットするようにする.
			Vector3 shotDirection = (targetPos - transform.position).normalized;
			if (Physics.Raycast (transform.position, shotDirection, out hinderinghit, maxrange, wcontrol.mask)){
				targetObject = hinderinghit.collider.gameObject;
				targetPos = hinderinghit.point;
			}
			
			//相手にダメージ判定があればレティクルを赤くし.
			//ダメージを与える.
			Hit dam = targetObject.GetComponentInParent<Hit>();
			if (dam != null){
				dam.TakeDamage(damage, unit);
				StopCoroutine("HitMark");
				StartCoroutine("HitMark");
			}
		} else {
			targetObject = null;
			targetPos = ray.GetPoint(maxrange);
		}
		RecoilAndDisperse ();
		RemainingLoads(2);
	}
	
	[RPC]
	protected void MakeShots(){
		transform.BroadcastMessage("MakeShot",targetPos);
	}
}

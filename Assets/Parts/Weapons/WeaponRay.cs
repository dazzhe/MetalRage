using UnityEngine;
using System.Collections;

public class WeaponRay : MonoBehaviour{
	private GameObject targetObject;
	public Vector3 targetPos;
	private Ray ray;
	private RaycastHit hit;
	private RaycastHit hinderinghit;
	public WeaponComponent component = new WeaponComponent();
	public WeaponParam param = new WeaponParam();

	public void RayShot(){
		float r = Random.value * param.mindispersion * component.wcontrol.desiredDispersion;
		float theta = Random.value * 2 * Mathf.PI;
		Vector3 shotpoint = component.wcontrol.center + new Vector3(r * Mathf.Cos (theta), r * Mathf.Sin (theta), 0);
		ray = Camera.main.ScreenPointToRay(shotpoint);
		if (Physics.Raycast(ray, out hit, param.maxrange, component.wcontrol.mask)) {
			targetPos = hit.point;
			targetObject = hit.collider.gameObject;
			
			//狙っているオブジェクトと機体の間に障害物があった場合.
			//障害物で弾がヒットするようにする.
			Vector3 shotDirection = (targetPos - transform.position).normalized;
			if (Physics.Raycast (transform.position, shotDirection, out hinderinghit, param.maxrange, component.wcontrol.mask)){
				targetObject = hinderinghit.collider.gameObject;
				targetPos = hinderinghit.point;
			}
			
			//相手にダメージ判定があればレティクルを赤くし.
			//ダメージを与える.
			Hit dam = targetObject.GetComponentInParent<Hit>();
			if (dam != null && targetObject.layer == 10){
				dam.TakeDamage(param.damage, component.unit.name);
				SendMessage("HitMark");
			}
		} else {
			targetObject = null;
			targetPos = ray.GetPoint(param.maxrange);
		}
	}
}

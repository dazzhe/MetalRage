using UnityEngine;
using System.Collections;

public class WeaponRay : MonoBehaviour
{
	public WeaponComponent component = new WeaponComponent();
	public WeaponParam param = new WeaponParam();
	public Vector3 targetPos;

	public void RayShot()
	{
		Ray ray = Camera.main.ScreenPointToRay(CalculateShotPoint());
		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit, param.maxrange, component.wcontrol.mask)) {
			targetPos = ray.GetPoint(param.maxrange);
			return;
		}

		//狙っているオブジェクトと機体の間に障害物があった場合.
		//障害物で弾がヒットするようにする.
		RaycastHit hinderingHit;
		Vector3 shotDirection = (hit.point - transform.position).normalized;
		if (Physics.Raycast(transform.position, shotDirection, out hinderingHit, param.maxrange, component.wcontrol.mask)) {
			hit = hinderingHit;
		}

		targetPos = hit.point;
		GameObject targetObject = hit.collider.gameObject;

		//相手にダメージ判定があればレティクルを赤くし.
		//ダメージを与える.
		Hit dam = targetObject.GetComponentInParent<Hit>();
		if (dam != null && targetObject.layer == 10) {
			dam.TakeDamage(param.damage, component.unit.name);
			component.wcontrol.HitMark();
		}
	}

	private Vector3 CalculateShotPoint()
	{
		float r = Random.value * param.mindispersion * component.wcontrol.desiredDispersion;
		float theta = Random.value * 2 * Mathf.PI;
		return component.wcontrol.center + new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0);
	}
}

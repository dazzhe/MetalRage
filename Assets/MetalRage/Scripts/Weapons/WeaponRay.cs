using UnityEngine;

public class WeaponRay : MonoBehaviour {
    public WeaponComponent component = new WeaponComponent();
    public WeaponParam param = new WeaponParam();
    public Vector3 targetPos;

    public void RayShot() {
        Ray ray = Camera.main.ScreenPointToRay(CalculateShotPoint());
        var layerMask = LayerMask.GetMask("Player");
        layerMask = ~layerMask;
        if (!Physics.Raycast(ray, out RaycastHit hit, this.param.maxrange, layerMask)) {
            this.targetPos = ray.GetPoint(this.param.maxrange);
            return;
        }
        // Obstacle check
        Vector3 shotDirection = (hit.point - this.transform.position).normalized;
        if (Physics.Raycast(this.transform.position, shotDirection, out RaycastHit hinderingHit, this.param.maxrange, layerMask)) {
            hit = hinderingHit;
        }
        this.targetPos = hit.point;
        GameObject targetObject = hit.collider.gameObject;
        Hit dam = targetObject.GetComponentInParent<Hit>();
        if (dam != null && targetObject.layer == LayerMask.NameToLayer("Enemy")) {
            dam.TakeDamage(this.param.damage, this.component.unit.name);
            this.component.wcontrol.HitMark();
        }
    }

    private Vector3 CalculateShotPoint() {
        float r = Random.value * this.param.minDispersion * this.component.wcontrol.Dispersion;
        float theta = Random.value * 2 * Mathf.PI;
        return this.component.wcontrol.Center + new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0);
    }
}

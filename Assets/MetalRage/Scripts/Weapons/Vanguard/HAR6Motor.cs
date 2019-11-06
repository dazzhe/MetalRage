using System.Collections;
using UnityEngine;

public class HAR6Motor : MonoBehaviour {
    [SerializeField]
    private GameObject bulletPrefab = default;

    private GameObject muzzleFlash;

    private void Awake() {
        this.muzzleFlash = this.transform.Find("MuzzleFlash").gameObject;
        this.muzzleFlash.SetActive(false);
    }

    private void MakeShot(Vector3 targetPos) {
        GetComponent<AudioSource>().Play();
        StartCoroutine(ShowMuzzleFlash());
        CreateBullet(targetPos);
    }

    private IEnumerator ShowMuzzleFlash() {
        if (this.muzzleFlash != null) {
            this.muzzleFlash.SetActive(true);
        }

        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));
        if (this.muzzleFlash != null) {
            this.muzzleFlash.SetActive(false);
        }
    }

    private void CreateBullet(Vector3 endPosition) {
        GameObject bulletObj = Instantiate(this.bulletPrefab, this.transform.position, this.transform.rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.EndPosition = endPosition;
        bullet.StartPosition = this.transform.position;
        bullet.IgnoreCollider = this.transform.parent.parent.parent.parent.GetComponent<Collider>();
    }
}
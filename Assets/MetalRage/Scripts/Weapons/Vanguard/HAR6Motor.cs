using System.Collections;
using UnityEngine;

public class HAR6Motor : MonoBehaviour {
    [SerializeField]
    private GameObject bulletPrefab;

    private GameObject muzzleFlash;

    private void Awake() {
        this.muzzleFlash = this.transform.Find("MuzzleFlash").gameObject;
        this.muzzleFlash.SetActive(false);
    }

    private void MakeShot(Vector3 targetPos) {
        GetComponent<AudioSource>().Play();
        StartCoroutine(ShowMuzzleFlash());
        StartCoroutine(CreateBullet(targetPos));
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

    private IEnumerator CreateBullet(Vector3 targetPos) {
        GameObject bulletObj = Instantiate(this.bulletPrefab, this.transform.position, this.transform.rotation) as GameObject;
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.targetPos = targetPos;
        bullet.originPos = this.transform.position;
        bullet.IgnoreCollider = this.transform.parent.parent.parent.parent.GetComponent<Collider>();
        yield return null;
    }
}
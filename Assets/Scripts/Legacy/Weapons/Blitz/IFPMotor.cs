//using UnityEngine;
//using System.Collections;

//public class IFPMotor : MonoBehaviour {
//	private GameObject MuzzleFlash;
//	public GameObject bullet;
	
//	void Awake(){
//		MuzzleFlash = transform.Find("MuzzleFlash").gameObject	;
//		MuzzleFlash.SetActive(false);
//	}
	
//	void MakeShot(Vector3 targetPos){
//		GetComponent<AudioSource>().Play();
//		StartCoroutine(this.ShowMuzzleFlash());
//		StartCoroutine (this.CreateBullet(targetPos));
//	}
	
//	IEnumerator ShowMuzzleFlash(){
//		if (MuzzleFlash != null)
//			MuzzleFlash.SetActive(true);
//		yield return new WaitForSeconds(Random.Range (0.01f,0.03f));
//		if (MuzzleFlash != null)
//			MuzzleFlash.SetActive(false);
//	}
	
//	IEnumerator CreateBullet(Vector3 targetPos){
//		GameObject bul = GameObject.Instantiate(bullet,transform.position,transform.rotation) as GameObject;
//		Bullet _bul = bul.GetComponent<Bullet>();
//		_bul.EndPosition = targetPos;
//		_bul.StartPosition = transform.position;
//		yield return null;
//	}
//}
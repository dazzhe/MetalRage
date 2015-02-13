using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {
	private float maxSpeed = 50f;
	private float currentSpeed = 0f;
	public int damage = 120;
	public string shooterPlayer;
	public WeaponControl wcontrol;

	private Vector3 direction;
	public Vector3 targetPos;
	private PhotonView myPV;

	void Start () {
		direction = transform.forward;
		myPV = GetComponent<PhotonView>();
	}
	
	void FixedUpdate () {
		currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, 5f * Time.deltaTime);
		rigidbody.velocity = currentSpeed * direction;
	}

	void OnCollisionEnter(Collision col){
		if (!myPV.isMine)
			return;
		Hit dam = col.gameObject.GetComponentInParent<Hit>();
		if (dam != null && col.gameObject.layer == 10) {
			dam.TakeDamage(50, shooterPlayer);
			wcontrol.HitMark();
		}
		Collider[] collidersInSphere = Physics.OverlapSphere(transform.position, 7f);
		foreach (Collider scol in collidersInSphere){
			Hit dam2 = scol.gameObject.GetComponentInParent<Hit>();
			if (dam2 != null && (scol.gameObject.layer == 10 || scol.gameObject.layer == 8)) {
				dam2.TakeDamage(50, shooterPlayer);
				wcontrol.HitMark();
			}
		}
		myPV.RPC("Explosion",PhotonTargets.All,transform.position);
		PhotonNetwork.Destroy(gameObject);
	}
	[RPC]
	void Explosion(Vector3 position){
		GameObject.Instantiate(Resources.Load("Explosion"), position, Quaternion.identity);
	}
}

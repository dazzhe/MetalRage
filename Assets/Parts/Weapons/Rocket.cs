using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {
	private float maxSpeed = 200f;
	private float currentSpeed = 0f;

	[System.NonSerialized]
	public int damage = 100;

	public string shooterPlayer;
	public WeaponControl wcontrol;
	private float radiusOfExplosion = 8f;
	private Vector3 direction;
	public Vector3 targetPos;
	private PhotonView myPV;

	void Start () {
		direction = transform.forward;
		myPV = GetComponent<PhotonView>();
	}
	
	void FixedUpdate () {
		currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, 5f * Time.deltaTime);
		GetComponent<Rigidbody>().velocity = currentSpeed * direction;
	}

	void OnCollisionEnter(Collision col){
		if (!myPV.isMine)
			return;
		Collider[] collidersInSphere = Physics.OverlapSphere(transform.position, radiusOfExplosion);
		foreach (Collider scol in collidersInSphere){
			Hit dam = scol.gameObject.GetComponentInParent<Hit>();
			if (dam != null && (scol.gameObject.layer == 10 || scol.gameObject.layer == 8)) {

				float distance = Mathf.Clamp((transform.position - scol.gameObject.transform.position).magnitude,
				                             3f, radiusOfExplosion);
				//distance = 3 -> correctDamage = damage (distance between the rocket and surface is about 0)
				//distance = rOE -> correctDamage = damage * 0.2
				float correctDamage = (float)damage
					* ( -0.8f / (radiusOfExplosion - 3f) * (distance - 3f) + 1f );
				dam.TakeDamage(Mathf.FloorToInt(correctDamage), shooterPlayer);
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

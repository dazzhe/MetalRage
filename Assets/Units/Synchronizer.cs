using UnityEngine;
using System.Collections;

public class Synchronizer : Photon.MonoBehaviour {
	UnitMotor motor;
	WeaponControl wcontrol;
	private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
	private int correctRotationX = 0;
	private int correctRotationY = 0;
	//private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this

	void Start(){
		motor = GetComponent<UnitMotor>();
		wcontrol = GetComponent<WeaponControl>();
	}

	void Update(){
		if (!photonView.isMine){
			transform.position
				= Vector3.Lerp(transform.position,
				               correctPlayerPos,
				               Time.deltaTime * 10);
			if (correctRotationX < 90 && motor.rotationX > 270f)
				correctRotationX += 360;
			if (correctRotationX > 270 && motor.rotationX < 90f)
				correctRotationX -= 360;

			motor.rotationX
				= Mathf.Lerp(motor.rotationX,
				             correctRotationX,
				             Time.deltaTime * 20);
			wcontrol.rotationY
				= Mathf.Lerp (wcontrol.rotationY,
				              correctRotationY,
				              Time.deltaTime * 20);
			if (motor.rotationX > 360f)
				motor.rotationX -= 360f;
			if (motor.rotationX < 0f)
				motor.rotationX += 360f;

			//transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting){
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(Mathf.FloorToInt(motor.rotationX));
			stream.SendNext(Mathf.FloorToInt(wcontrol.rotationY));
		}
		else{
			// Network player, receive data
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctRotationX = (int)stream.ReceiveNext();
			correctRotationY = (int)stream.ReceiveNext();
		}
	}
}
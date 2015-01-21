using UnityEngine;
using System.Collections;
//変数の同期を取る.
public class Synchronizer : Photon.MonoBehaviour {
	UnitMotor motor;
	WeaponControl wcontrol;
	private Vector3 correctPlayerPos = Vector3.zero;
	private short correctRotationX = 0;
	private byte correctRotationY = 0;

	void Start(){
		motor = GetComponent<UnitMotor>();
		wcontrol = GetComponent<WeaponControl>();
	}

	void Update(){
		if (!photonView.isMine){
			//入力情報の同期だけでは位置のずれが生じるため.
			//各機体の位置を同期する.
			//カクツキを防ぐためLerpしている.
			transform.position
				= Vector3.Lerp(transform.position,
				               correctPlayerPos,
				               Time.deltaTime * 10);

			float correctRotationX = ((float)this.correctRotationX+18000)/100;
			float correctRotationY = ((float)this.correctRotationY-120)/2;
			//逆回転を防ぐため回転核が360→0や0→360と変化した際Lerp先を修正する.
			//これでも100→350(0経由)などのように変化した場合逆にLerpしてしまうので修正が必要?.
			if (correctRotationX < 90f && motor.rotationX > 270f)
				correctRotationX += 360f;
			if (correctRotationX > 270f && motor.rotationX < 90f)
				correctRotationX -= 360f;

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
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if (stream.isWriting){
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext((short)(motor.rotationX*100-18000));
			stream.SendNext((byte)(wcontrol.rotationY*2+120));
		}
		else{
			// Network player, receive data
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			//通信量を減らすためサイズを減らす.
			//RotationXは0~360,RotationYは-60~60の範囲で動く.
			//なぜかushortとsbyteを使うとエラーが出た.
			correctRotationX = (short)stream.ReceiveNext();
			correctRotationY = (byte)stream.ReceiveNext();
		}
	}
}
using UnityEngine;

public class Synchronizer : Photon.MonoBehaviour, IPunObservable {
    private UnitMotor motor;
    private WeaponControl wcontrol;
    private Vector3 correctPlayerPos;
    private short correctRotationX = 0;
    private byte correctRotationY = 0;

    private void Awake() {
        this.motor = GetComponent<UnitMotor>();
        this.wcontrol = GetComponent<WeaponControl>();
    }

    private void Update() {
        if (!this.photonView.isMine) {
            //入力情報の同期だけでは位置のずれが生じるため.
            //各機体の位置を同期する.
            //カクツキを防ぐためLerpしている.
            this.transform.position
                = Vector3.Lerp(this.transform.position,
                               this.correctPlayerPos,
                               Time.deltaTime * 10);

            float correctRotationX = ((float)this.correctRotationX + 18000) / 100;
            float correctRotationY = ((float)this.correctRotationY - 120) / 2;
            //逆回転を防ぐため回転核が360→0や0→360と変化した際Lerp先を修正する.
            //これでも100→350(0経由)などのように変化した場合逆にLerpしてしまうので修正が必要?.
            if (correctRotationX < 90f && this.motor.rotationX > 270f) {
                correctRotationX += 360f;
            }

            if (correctRotationX > 270f && this.motor.rotationX < 90f) {
                correctRotationX -= 360f;
            }

            this.motor.rotationX
                = Mathf.Lerp(this.motor.rotationX,
                             correctRotationX,
                             Time.deltaTime * 20);
            this.wcontrol.rotationY
                = Mathf.Lerp(this.wcontrol.rotationY,
                              correctRotationY,
                              Time.deltaTime * 20);
            if (this.motor.rotationX > 360f) {
                this.motor.rotationX -= 360f;
            }

            if (this.motor.rotationX < 0f) {
                this.motor.rotationX += 360f;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // We own this player: send the others our data
            stream.SendNext(this.transform.position);
            stream.SendNext((short)(this.motor.rotationX * 100 - 18000));
            stream.SendNext((byte)(this.wcontrol.rotationY * 2 + 120));
        } else {
            // Network player, receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            //通信量を減らすためサイズを減らす.
            //RotationXは0~360,RotationYは-60~60の範囲で動く.
            this.correctRotationX = (short)stream.ReceiveNext();
            this.correctRotationY = (byte)stream.ReceiveNext();
        }
    }
}
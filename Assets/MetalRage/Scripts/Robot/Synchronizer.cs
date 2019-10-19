using UnityEngine;

public class Synchronizer : Photon.MonoBehaviour, IPunObservable {
    private UnitMotor motor;
    private Robot wcontrol;
    private Vector3 correctPlayerPos;
    private short correctYaw = 0;
    private byte correctPitch = 0;

    private void Awake() {
        this.motor = GetComponent<UnitMotor>();
        this.wcontrol = GetComponent<Robot>();
    }

    private void Update() {
        if (this.photonView.isMine) {
            return;
        }
        var isTooFarFromCorrectPosition = Vector3.Distance(this.transform.position, this.correctPlayerPos) > 10f;
        this.transform.position = isTooFarFromCorrectPosition
            ? this.correctPlayerPos
            : Vector3.Lerp(this.transform.position, this.correctPlayerPos, Time.deltaTime * 10f);
        float correctRotationX = ((float)this.correctYaw + 18000) / 100;
        float correctRotationY = ((float)this.correctPitch - 120) / 2;
        if (correctRotationX < 90f && this.motor.yaw > 270f) {
            correctRotationX += 360f;
        }
        if (correctRotationX > 270f && this.motor.yaw < 90f) {
            correctRotationX -= 360f;
        }
        this.motor.yaw
            = Mathf.Lerp(this.motor.yaw,
                         correctRotationX,
                         Time.deltaTime * 20f);
        this.wcontrol.RotationY
            = Mathf.Lerp(this.wcontrol.RotationY,
                          correctRotationY,
                          Time.deltaTime * 20f);
        if (this.motor.yaw > 360f) {
            this.motor.yaw -= 360f;
        }
        if (this.motor.yaw < 0f) {
            this.motor.yaw += 360f;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // Send data
            stream.SendNext(this.transform.position);
            // Compress RotationX to 0~360 and RotationY to -60~60.
            stream.SendNext((short)(this.motor.yaw * 100 - 18000));
            stream.SendNext((byte)(this.wcontrol.RotationY * 2 + 120));
        } else {
            // Receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctYaw = (short)stream.ReceiveNext();
            this.correctPitch = (byte)stream.ReceiveNext();
        }
    }
}

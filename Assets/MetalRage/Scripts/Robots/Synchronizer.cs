using UnityEngine;

public class Synchronizer : Photon.MonoBehaviour, IPunObservable {
    private UnitMotor motor;
    private WeaponControl wcontrol;
    private Vector3 correctPlayerPos;
    private short correctYaw = 0;
    private byte correctPitch = 0;

    private void Awake() {
        this.motor = GetComponent<UnitMotor>();
        this.wcontrol = GetComponent<WeaponControl>();
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
        if (correctRotationX < 90f && this.motor.rotationX > 270f) {
            correctRotationX += 360f;
        }
        if (correctRotationX > 270f && this.motor.rotationX < 90f) {
            correctRotationX -= 360f;
        }
        this.motor.rotationX
            = Mathf.Lerp(this.motor.rotationX,
                         correctRotationX,
                         Time.deltaTime * 20f);
        this.wcontrol.RotationY
            = Mathf.Lerp(this.wcontrol.RotationY,
                          correctRotationY,
                          Time.deltaTime * 20f);
        if (this.motor.rotationX > 360f) {
            this.motor.rotationX -= 360f;
        }
        if (this.motor.rotationX < 0f) {
            this.motor.rotationX += 360f;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // Send data
            stream.SendNext(this.transform.position);
            // Compress RotationX to 0~360 and RotationY to -60~60.
            stream.SendNext((short)(this.motor.rotationX * 100 - 18000));
            stream.SendNext((byte)(this.wcontrol.RotationY * 2 + 120));
        } else {
            // Receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctYaw = (short)stream.ReceiveNext();
            this.correctPitch = (byte)stream.ReceiveNext();
        }
    }
}

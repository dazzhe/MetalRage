using UnityEngine;

public enum Button : byte {
    Fire1 = 1,
    Fire2 = 1 << 1,
    Reload = 1 << 2,
    MainWeapon = 1 << 3,
    LeftWeapon = 1 << 4,
}

public class UserCommand : Photon.MonoBehaviour, IPunObservable {
    private bool IsControllable => UIManager.Instance.MenuUI.ActiveWindowLevel == 0;

    public bool SelectMainWeapon {
        get {
            return this.IsControllable && Input.GetButtonDown("MainWeapon");
        }
    }

    public bool SelectLeftWeapon => this.IsControllable && Input.GetButtonDown("LeftWeapon");
    public bool SelectRightWeapon => this.IsControllable && Input.GetButtonDown("RightWeapon");
    public bool Reload => this.IsControllable && Input.GetButtonDown("Reload");
    public bool Fire1 => this.IsControllable && Input.GetButton("Fire1");
    public bool Fire2 => this.IsControllable && Input.GetButtonDown("Fire2");
    public float Pitch => this.IsControllable ? Input.GetAxis("Mouse Y") * Configuration.Sensitivity.GetFloat() : 0f;
    public float Yaw => this.IsControllable ? Input.GetAxis("Mouse X") * Configuration.Sensitivity.GetFloat() : 0f;

    public struct InputFlags {

    }

    private void Start() {
        this.photonView.ObservedComponents.Add(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            //stream.SendNext()
        }
        if (stream.isReading) {

        }
    }
}

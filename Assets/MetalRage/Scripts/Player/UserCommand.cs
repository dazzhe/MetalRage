using UnityEngine;

public class UserCommand : Photon.MonoBehaviour {
    public bool SelectMainWeapon => Input.GetButtonDown("MainWeapon");
    public bool SelectLeftWeapon => Input.GetButtonDown("LeftWeapon");
    public bool SelectRightWeapon => Input.GetButtonDown("RightWeapon");
    public float Pitch => Input.GetAxis("Mouse Y") * Configuration.Sensitivity.GetFloat();
    public float Yaw => Input.GetAxis("Mouse X") * Configuration.Sensitivity.GetFloat();
    public bool Reload => Input.GetButtonDown("Reload");
    public bool Fire1 => Input.GetButton("Fire1");
    public bool Fire2 => Input.GetButtonDown("Fire2");
}

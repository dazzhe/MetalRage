using UnityEngine;

public class WeaponManager : MonoBehaviour {
    private GameObject weapon;

    private void Awake() {
        this.weapon = this.transform.GetChild(0).gameObject;
    }

    public void SendEnable() {
        this.weapon.SendMessage("Enable");
    }

    public void SendDisable() {
        this.weapon.SendMessage("Disable");
    }
}

using System.Collections;
using UnityEngine;

public class AMRLJ : Weapon {
    private Animation anim;

    private void Awake() {
        this.Ammo.MagazineSize = 4;
        this.Ammo.ReserveBulletCount = 4;
        this.Ammo.Reload();
        this.param.recoil = Vector2.zero;
        this.Spread.MinAngle = 10f; // For showing gunsight. Not actually disparse.
        this.Spread.GrowRate = 0.3f;
        this.param.coolDownTime = 1.5f;
        this.anim = GetComponent<Animation>();
        Initialize();
    }

    private void LateUpdate() {
        if (InputSystem.GetButton(MechCommandButton.Fire1) && !this.Ammo.IsMagazineEmpty && !this.param.isCoolDown) {
            StartCoroutine(Shot());
        }
        this.Crosshair.UpdateConeOfFire(this.Spread);
    }

    private IEnumerator Shot() {
        this.param.isCoolDown = true;
        for (int i = 0; i <= 1; ++i) {
            if (this.Ammo.LoadedBulletCount == 0) {
                yield break;
            }
            MakeShot(this.ray.Raycast(Vector2.zero).point);
            RecoilAndSpread();
            ConsumeBullets(1);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(this.param.coolDownTime);
        this.param.isCoolDown = false;
    }

    private void MakeShot(Vector3 targetPos) {
        StartCoroutine(CreateBullet(targetPos));
    }

    private IEnumerator CreateBullet(Vector3 targetPos) {
        Quaternion rot = Quaternion.LookRotation(targetPos - this.transform.position);
        GameObject rocketObject = PhotonNetwork.Instantiate("Rocket", this.transform.parent.position, rot, 0);
        Rocket rocket = rocketObject.GetComponent<Rocket>();
        rocket.shooterPlayer = this.unitMotor.gameObject.name;
        rocket.wcontrol = this.robot;
        yield return null;
    }

    public override void Select() {
        this.anim["SettingUp"].speed = 1;
        this.anim.Play("SettingUp");
        base.Select();
    }

    public override void Unselect() {
        if (this.anim["SettingUp"].time == 0) {
            this.anim["SettingUp"].time = this.anim["SettingUp"].length;
        }
        this.anim["SettingUp"].speed = -3;
        this.anim.Play("SettingUp");
        base.Unselect();
    }
}

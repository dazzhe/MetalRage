using System.Collections;
using UnityEngine;

public class AMRLJ : Weapon {
    private Animation anim;

    private void Awake() {
        this.Ammo.MagazineSize = 4;
        this.Ammo.ReserveBulletCount = 4;
        this.Ammo.Reload();
        this.param.recoilY = 0f;
        this.param.minDispersion = 10f; // For showing gunsight. Not actually disparse.
        this.param.dispersionGrow = 3f;
        this.param.interval = 1.5f;
        this.anim = GetComponent<Animation>();
        Init();
    }

    private void LateUpdate() {
        if (this.component.wcontrol.inputShot1 && !this.Ammo.IsMagazineEmpty && !this.param.cooldown) {
            StartCoroutine(Shot());
        }
        this.Gunsight.extent = this.param.minDispersion * this.component.wcontrol.Dispersion * 2;
    }

    private IEnumerator Shot() {
        this.param.cooldown = true;
        for (int i = 0; i <= 1; ++i) {
            if (this.Ammo.LoadedBulletCount == 0) {
                yield break;
            }
            MakeShot(this.component.wcontrol.targetPos);
            RecoilAndDisperse();
            ConsumeBullets(1);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(this.param.interval);
        this.param.cooldown = false;
    }

    private void MakeShot(Vector3 targetPos) {
        StartCoroutine(CreateBullet(targetPos));
    }

    private IEnumerator CreateBullet(Vector3 targetPos) {
        Quaternion rot = Quaternion.LookRotation(targetPos - this.transform.position);
        GameObject rocketObject = PhotonNetwork.Instantiate("Rocket", this.transform.parent.position, rot, 0);
        Rocket rocket = rocketObject.GetComponent<Rocket>();
        rocket.shooterPlayer = this.component.unit.name;
        rocket.wcontrol = this.component.wcontrol;
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

using System.Collections;
using UnityEngine;

public class AMRLJ : Weapon {
    private Animation anim;

    private void Awake() {
        this.param.magazine = 4;
        this.param.recoilY = 0f;
        this.param.minDispersion = 10f; //only for showing sight
        this.param.dispersionGrow = 3f;
        this.param.interval = 1.5f;
        this.anim = GetComponent<Animation>();
        Init();
    }

    private void LateUpdate() {
        if (this.component.wcontrol.inputShot1 && this.param.load != 0 && !this.param.cooldown && this.param.canShot) {
            StartCoroutine(Shot());
        }

        this.sight.extent = this.param.minDispersion * this.component.wcontrol.desiredDispersion * 2;
    }

    private IEnumerator Shot() {
        this.param.cooldown = true;
        for (int i = 0; i <= 1; i++) {
            if (this.param.load == 0) {
                yield break;
            }

            MakeShot(this.component.wcontrol.targetPos);
            RecoilAndDisperse();
            RemainingLoads(1);
            if (i == 1) {
                break;
            }

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
        GameObject rocket = PhotonNetwork.Instantiate("Rocket", this.transform.parent.position, rot, 0);
        Rocket _rocket = rocket.GetComponent<Rocket>();
        _rocket.shooterPlayer = this.component.unit.name;
        _rocket.wcontrol = this.component.wcontrol;
        yield return null;
    }

    protected override void Enable() {
        this.anim["SettingUp"].speed = 1;
        this.anim.Play("SettingUp");
        base.Enable();
    }

    protected override void Disable() {
        if (this.anim["SettingUp"].time == 0) {
            this.anim["SettingUp"].time = this.anim["SettingUp"].length;
        }
        this.anim["SettingUp"].speed = -3;
        this.anim.Play("SettingUp");
        base.Disable();
    }
}
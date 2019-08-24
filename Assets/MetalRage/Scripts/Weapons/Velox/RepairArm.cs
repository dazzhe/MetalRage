using System.Collections;
using UnityEngine;

public class RepairArm : Weapon {
    private Animation _animation;
    private AudioSource repair;
    private Status targetingUnitStatus;

    private void Awake() {
        this.param.magazine = 100;
        this.param.damage = -40;
        this.param.recoilY = 0f;//反動.
        this.param.minDispersion = 0f;//ばらつき.
        this.param.dispersionGrow = 0f;
        this.param.maxrange = 1000;
        this._animation = GetComponent<Animation>();
        Init();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        this.repair = audioSources[1];
        if (this.component.myPV.isMine) {
            StartCoroutine("Remain");
        }
    }

    private IEnumerator Remain() {
        while (true) {
            if (this.param.load < this.param.magazine && this._animation.IsPlaying("Default")) {
                this.param.load += 2;
                if (this.param.load > this.param.magazine) {
                    this.param.load = this.param.magazine;
                }

                UIManager.Instance.StatusUI.SetMagazine(this.param.load);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void LateUpdate() {
        this.targetingUnitStatus = this.component.wcontrol.targetObject.GetComponent<Status>();
        if (this.targetingUnitStatus != null && this.component.wcontrol.targetObject.layer == 9) {
            UIManager.Instance.StatusUI.TargetingFriend = this.targetingUnitStatus;
        } else {
            UIManager.Instance.StatusUI.TargetingFriend = null;
        }
        if (this.component.wcontrol.inputShot1) {
            if (this._animation.IsPlaying("Default")) {
                this._animation.CrossFade("PreRepair", 0.5f);
            } else if (!this._animation.IsPlaying("PreRepair") && !this._animation.IsPlaying("Repair")) {
                this.component.myPV.RPC("RepairRPC", PhotonTargets.All);
            }
        } else {
            if (!this.component.myPV.isMine && this._animation.IsPlaying("Default")) {
                this.component.myPV.RPC("Default", PhotonTargets.All);
            } else {
                this._animation.Play("Default");
            }
        }
    }

    private void Repair() {
        this.repair.PlayOneShot(this.repair.clip);
        if (this.component.myPV.isMine) {
            Hit hit = this.component.wcontrol.targetObject.GetComponentInParent<Hit>();
            if (hit != null && this.component.wcontrol.targetObject.layer == 9 &&
                this.targetingUnitStatus.HP != this.targetingUnitStatus.maxHP && this.param.load >= 15
                && (this.component.wcontrol.targetObject.transform.position
                     - this.transform.position).magnitude <= 4f) {
                hit.TakeDamage(this.param.damage, this.component.unit.name);
                RemainingLoads(15);
            }
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        UIManager.Instance.StatusUI.TargetingFriend = null;
    }

    [PunRPC]
    private void Default() {
        this._animation.Play("Default");
    }

    [PunRPC]
    private void RepairRPC() {
        this._animation.Play("Repair");
    }
}
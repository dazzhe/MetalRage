using System.Collections;
using UnityEngine;

public class RepairArm : Weapon {
    private static readonly string kIsRepairingKey = "IsRepairing";

    [SerializeField]
    private AudioClip repairClip;

    private Animator animator;
    private new AudioSource audio;
    private Status targetingUnitStatus;
    private RepairArmReticle reticle;

    private void Awake() {
        this.Ammo.MagazineSize = 100;
        this.Ammo.ReserveBulletCount = 100;
        this.Ammo.Reload();
        this.param.damage = -40;
        this.param.recoilY = 0f;
        this.param.minDispersion = 0f;
        this.param.dispersionGrow = 0f;
        this.param.maxrange = 1000;
        this.animator = GetComponent<Animator>();
        this.audio = GetComponent<AudioSource>();
        this.reticle = this.Gunsight.gameObject.GetComponent<RepairArmReticle>();
        Init();
        if (this.component.myPV.isMine) {
            StartCoroutine(BulletRefillLoop());
        }
    }

    private IEnumerator BulletRefillLoop() {
        while (true) {
            if (!this.Ammo.IsMagazineFull && !this.animator.GetBool(kIsRepairingKey)) {
                this.Ammo.Supply(2);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void LateUpdate() {
        this.targetingUnitStatus = this.component.wcontrol.TargetObject.GetComponent<Status>();
        if (this.targetingUnitStatus != null && this.component.wcontrol.TargetObject.layer == 9) {
            UIManager.Instance.StatusUI.TargetingFriend = this.targetingUnitStatus;
        } else {
            UIManager.Instance.StatusUI.TargetingFriend = null;
        }
        SetRepairingState(this.component.wcontrol.inputShot1);
    }

    private void SetRepairingState(bool value) {
        if (value == this.animator.GetBool(kIsRepairingKey)) {
            return;
        }
        PhotonNetwork.RPC(this.component.myPV, "SetRepairingStateOnNetwork", PhotonTargets.All, false, value);
    }

    [PunRPC]
    private void SetRepairingStateOnNetwork(bool value) {
        this.animator.SetBool(kIsRepairingKey, value);
    }

    public void Repair() {
        this.audio.PlayOneShot(this.audio.clip);
        if (this.component.myPV.isMine) {
            Hit hit = this.component.wcontrol.TargetObject.GetComponentInParent<Hit>();
            if (hit != null && this.component.wcontrol.TargetObject.layer == 9 &&
                this.targetingUnitStatus.HP != this.targetingUnitStatus.MaxHP && this.Ammo.LoadedBulletCount >= 15
                && (this.component.wcontrol.TargetObject.transform.position
                     - this.transform.position).magnitude <= 4f) {
                hit.TakeDamage(this.param.damage, this.component.unit.name);
                ConsumeBullets(15);
            }
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (UIManager.Instance == null) {
            return;
        }
        UIManager.Instance.StatusUI.TargetingFriend = null;
    }
}

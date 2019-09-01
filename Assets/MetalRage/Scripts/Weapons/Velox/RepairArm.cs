using System.Collections;
using UnityEngine;

public class RepairArm : Weapon {
    private static readonly string kIsRepairingKey = "IsRepairing";

    [SerializeField]
    private AudioClip repairClip;

    private Animator animator;
    private new AudioSource audio;
    private Status targetingUnitStatus;

    private void Awake() {
        this.param.magazine = 100;
        this.param.damage = -40;
        this.param.recoilY = 0f;//反動.
        this.param.minDispersion = 0f;//ばらつき.
        this.param.dispersionGrow = 0f;
        this.param.maxrange = 1000;
        this.animator = GetComponent<Animator>();
        this.audio = GetComponent<AudioSource>();
        Init();
        if (this.component.myPV.isMine) {
            StartCoroutine(RegenerateRemainingBullet());
        }
    }

    private IEnumerator RegenerateRemainingBullet() {
        while (true) {
            if (this.param.load < this.param.magazine && !this.animator.GetBool(kIsRepairingKey)) {
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
            Hit hit = this.component.wcontrol.targetObject.GetComponentInParent<Hit>();
            if (hit != null && this.component.wcontrol.targetObject.layer == 9 &&
                this.targetingUnitStatus.HP != this.targetingUnitStatus.MaxHP && this.param.load >= 15
                && (this.component.wcontrol.targetObject.transform.position
                     - this.transform.position).magnitude <= 4f) {
                hit.TakeDamage(this.param.damage, this.component.unit.name);
                SetRemainingLoads(15);
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
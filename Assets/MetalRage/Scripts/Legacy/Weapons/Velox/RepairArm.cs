//using System.Collections;
//using UnityEngine;

//public class RepairArm : Weapon {
//    private static readonly string kIsRepairingKey = "IsRepairing";

//    [SerializeField]
//    private AudioClip repairClip;

//    private Animator animator;
//    private new AudioSource audio;
//    private MechStatus targetingUnitStatus;
//    private RepairArmReticle reticle;

//    private void Awake() {
//        this.Ammo.MagazineSize = 100;
//        this.Ammo.ReserveBulletCount = 100;
//        this.Ammo.Reload();
//        this.param.damage = -40;
//        this.param.recoil = Vector2.zero;
//        this.Spread.MinAngle = 0f;
//        this.Spread.GrowRate = 0f;
//        this.animator = GetComponent<Animator>();
//        this.audio = GetComponent<AudioSource>();
//        this.reticle = this.Crosshair.gameObject.GetComponent<RepairArmReticle>();
//        Initialize();
//        if (this.photonView.isMine) {
//            StartCoroutine(BulletRefillLoop());
//        }
//    }

//    private IEnumerator BulletRefillLoop() {
//        while (true) {
//            if (!this.Ammo.IsMagazineFull && !this.animator.GetBool(kIsRepairingKey)) {
//                this.Ammo.Supply(2);
//            }
//            yield return new WaitForSeconds(0.1f);
//        }
//    }

//    private void LateUpdate() {
//        this.targetingUnitStatus = this.robot.TargetObject.GetComponent<MechStatus>();
//        if (this.targetingUnitStatus != null && this.robot.TargetObject.layer == 9) {
//            UIManager.Instance.StatusUI.TargetingFriend = this.targetingUnitStatus;
//        } else {
//            UIManager.Instance.StatusUI.TargetingFriend = null;
//        }
//        SetRepairingState(InputSystem.GetButton(MechCommandButton.Fire1));
//    }

//    private void SetRepairingState(bool value) {
//        if (value == this.animator.GetBool(kIsRepairingKey)) {
//            return;
//        }
//        PhotonNetwork.RPC(this.photonView, "SetRepairingStateOnNetwork", PhotonTargets.All, false, value);
//    }

//    [PunRPC]
//    private void SetRepairingStateOnNetwork(bool value) {
//        this.animator.SetBool(kIsRepairingKey, value);
//    }

//    public void Repair() {
//        this.audio.PlayOneShot(this.audio.clip);
//        if (this.photonView.isMine) {
//            Hit hit = this.robot.TargetObject.GetComponentInParent<Hit>();
//            if (hit != null && this.robot.TargetObject.layer == 9 &&
//                this.targetingUnitStatus.HP != this.targetingUnitStatus.MaxHP && this.Ammo.LoadedBulletCount >= 15
//                && (this.robot.TargetObject.transform.position
//                     - this.transform.position).magnitude <= 4f) {
//                hit.TakeDamage(this.param.damage, this.unitMotor.name);
//                ConsumeBullets(15);
//            }
//        }
//    }

//    protected override void OnDestroy() {
//        base.OnDestroy();
//        if (UIManager.Instance == null) {
//            return;
//        }
//        UIManager.Instance.StatusUI.TargetingFriend = null;
//    }
//}

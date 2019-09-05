using System.Collections;
using UnityEngine;

public class WeaponControl : MonoBehaviour {
    private UnitMotor motor;
    private Gunsight[] gunsights;
    private Weapon[] weapons = new Weapon[3];

    public GameObject TargetObject { get; set; }
    public bool IsRecoiling { get; set; }
    public float RotationY { get; set; }
    public float BaseRotationY { get; set; }
    public Vector2 RecoilRotation { get; set; }
    public float DispersionRate { get; set; }
    public float Dispersion { get; set; }
    [System.NonSerialized]
    public Vector3 targetPos;
    [System.NonSerialized]
    public bool inputReload = false;
    [System.NonSerialized]
    public bool inputShot1 = false;
    [System.NonSerialized]
    public bool isBlitzMain = false;
    [System.NonSerialized]
    public bool inputShot2 = false;

    public Vector3 Center { get => new Vector3(Screen.width / 2, Screen.height / 2, 0); }

    private int selecedWeaponIndex = 0;  //0 = Main, 1 = Right, 2 = Left
    private RangeFloat elevationRange = new RangeFloat(-60f, 60f);
    private float recoilFixSpeed = 25f;

    private float DispersionCorrection() {
        switch (this.motor.characterState) {
        case UnitMotor.CharacterState.Idle:
            return 1f;
        case UnitMotor.CharacterState.Walking:
            return 1f;
        case UnitMotor.CharacterState.Boosting:
            return 1.3f;
        case UnitMotor.CharacterState.Braking:
            return 1.2f;
        case UnitMotor.CharacterState.Squatting:
            return 0.5f;
        case UnitMotor.CharacterState.Jumping:
            return 1.5f;
        default:
            return 1f;
        }
    }

    private void Start() {
        this.motor = GetComponent<UnitMotor>();
        this.weapons[0] = this.transform.Find("Offset/MainWeapon").GetComponentInChildren<Weapon>();
        this.weapons[1] = this.transform.Find("Offset/RightWeapon").GetComponentInChildren<Weapon>();
        this.weapons[2] = this.transform.Find("Offset/LeftWeapon").GetComponentInChildren<Weapon>();
        this.weapons[0].Select();
        this.gunsights = FindObjectsOfType(typeof(Gunsight)) as Gunsight[];
    }

    private void Update() {
        UIManager.Instance.AmmoUI.UpdateUI(this.weapons[this.selecedWeaponIndex].Ammo);
        if (UIManager.Instance.MenuUI.ActiveWindowLevel == 0) {
            WeaponSelect();
            this.inputReload = Input.GetButtonDown("Reload");
            this.inputShot1 = Input.GetButton("Fire1");
            this.inputShot2 = Input.GetButtonDown("Fire2");
            Elevation();
        } else {
            this.inputReload = false;
            this.inputShot1 = false;
            this.inputShot2 = false;
        }
        if (!this.isBlitzMain) {
            SetTarget();
        }
        SuppressRecoil();
        SuppressDispersion();
    }

    private void WeaponSelect() {
        if (Input.GetButtonDown("MainWeapon") && this.selecedWeaponIndex != 0) {
            this.weapons[this.selecedWeaponIndex].Unselect();
            this.weapons[0].Select();
            this.selecedWeaponIndex = 0;
        }
        if (Input.GetButtonDown("RightWeapon") && this.selecedWeaponIndex != 1) {
            this.weapons[this.selecedWeaponIndex].Unselect();
            this.weapons[1].Select();
            this.selecedWeaponIndex = 1;
        }
        if (Input.GetButtonDown("LeftWeapon") && this.selecedWeaponIndex != 2) {
            this.weapons[this.selecedWeaponIndex].Unselect();
            this.weapons[2].Select();
            this.selecedWeaponIndex = 2;
        }
    }

    private void Elevation() {
        this.BaseRotationY += Input.GetAxis("Mouse Y") * Configuration.sensitivity * this.motor.sensimag;
        this.BaseRotationY = Mathf.Clamp(this.BaseRotationY, this.elevationRange.Min, this.elevationRange.Max);
        this.RotationY = this.BaseRotationY + this.RecoilRotation.y;
    }

    private void SuppressRecoil() {
        if ((this.RecoilRotation.x == 0f && this.RecoilRotation.y == 0f) || this.IsRecoiling) {
            return;
        }
        var recoilRotation = this.RecoilRotation;
        if (this.RecoilRotation.y > 0f) {
            recoilRotation.y -= this.recoilFixSpeed * Time.deltaTime;
        }
        if (this.RecoilRotation.y < 0f) {
            recoilRotation.y = 0f;
        }
        if (this.RecoilRotation.x != 0) {
            recoilRotation.x = Mathf.Lerp(this.RecoilRotation.x, 0, 0.1f);
        }
        this.RecoilRotation = recoilRotation;
    }

    private void SuppressDispersion() {
        this.DispersionRate = Mathf.Clamp(this.DispersionRate - 3f * Time.deltaTime, 1, this.DispersionRate);
        this.Dispersion = this.DispersionRate * DispersionCorrection() * Screen.height * 0.001f;
    }

    private void SetTarget() {
        Ray ray = Camera.main.ScreenPointToRay(this.Center);
        var layerMask = LayerMask.GetMask("Player");
        layerMask = ~layerMask;
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask)) {
            this.targetPos = ray.GetPoint(1000f);
            return;
        }
        this.targetPos = hit.point;
        this.TargetObject = hit.collider.gameObject;
        if (this.TargetObject.layer == LayerMask.NameToLayer("Enemy")) {
            StopCoroutine(ShowEnemyName());
            StartCoroutine(ShowEnemyName());
        }
    }

    private IEnumerator ShowEnemyName() {
        UIManager.Instance.StatusUI.TargetingEnemyName = this.TargetObject.GetComponentInParent<FriendOrEnemy>().playerName;
        yield return new WaitForSeconds(0.2f);
        UIManager.Instance.StatusUI.TargetingEnemyName = "";
    }

    public void HitMark() {
        StopCoroutine(HitMarkRoutine());
        StartCoroutine(HitMarkRoutine());
    }

    private IEnumerator HitMarkRoutine() {
        foreach (Gunsight sight in this.gunsights) {
            sight.SetColor(Color.red);
        }
        yield return new WaitForSeconds(0.3f);
        foreach (Gunsight sight in this.gunsights) {
            sight.SetColor(Color.white);
        }
    }

    private void OnDestroy() {
        if (UIManager.Instance == null) {
            return;
        }
        UIManager.Instance.StatusUI.TargetingEnemyName = "";
    }
}

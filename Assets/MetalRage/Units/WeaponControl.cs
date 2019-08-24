using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponControl : MonoBehaviour {
    private UnitMotor motor;
    private Sight[] sights;
    private WeaponManager[] weapons = new WeaponManager[3];
    [System.NonSerialized]
    public GameObject targetObject;
    [System.NonSerialized]
    public bool isRecoiling = false;
    [System.NonSerialized]
    public float rotationY = 0F;
    [System.NonSerialized]
    public float normalrotationY = 0F;
    [System.NonSerialized]
    public float recoilrotationx = 0F;
    [System.NonSerialized]
    public float recoilrotationy = 0F;
    [System.NonSerialized]
    public float dispersionRate = 0;
    [System.NonSerialized]
    public float desiredDispersion;
    [System.NonSerialized]
    public Vector3 targetPos;
    [System.NonSerialized]
    public LayerMask mask;
    [System.NonSerialized]
    public Vector3 center = Vector3.zero;
    [System.NonSerialized]
    public bool inputReload = false;
    [System.NonSerialized]
    public bool inputShot1 = false;
    [System.NonSerialized]
    public bool isBlitzMain = false;
    [System.NonSerialized]
    public bool inputShot2 = false;

    private int enabledWeapon = 0;  //0 = Main, 1 = Right, 2 = Left
    private float minimumY = -60F;
    private float maximumY = 60F;
    private float recoilFixSpeed = 25f;

    private float DispersionCorrection() {
        switch (this.motor._characterState) {
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
        }
        return 1f;
    }

    private void Start() {
        this.motor = GetComponent<UnitMotor>();
        this.weapons[0] = this.transform.Find("Offset/MainWeapon").GetComponent<WeaponManager>();
        this.weapons[1] = this.transform.Find("Offset/RightWeapon").GetComponent<WeaponManager>();
        this.weapons[2] = this.transform.Find("Offset/LeftWeapon").GetComponent<WeaponManager>();
        this.weapons[0].SendEnable();

        this.sights = GameObject.FindObjectsOfType(typeof(Sight)) as Sight[];
        this.center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        //8番のレイヤー(操作している機体)を無視する.
        this.mask = 1 << 8;
        this.mask = ~this.mask;
    }

    private void Update() {
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

        RecoilControl();
        DispersionControl();
    }

    private void WeaponSelect() {
        if (Input.GetButtonDown("MainWeapon") && this.enabledWeapon != 0) {
            this.weapons[this.enabledWeapon].SendDisable();
            this.weapons[0].SendEnable();
            this.enabledWeapon = 0;
        }
        if (Input.GetButtonDown("RightWeapon") && this.enabledWeapon != 1) {
            this.weapons[this.enabledWeapon].SendDisable();
            this.weapons[1].SendEnable();
            this.enabledWeapon = 1;
        }
        if (Input.GetButtonDown("LeftWeapon") && this.enabledWeapon != 2) {
            this.weapons[this.enabledWeapon].SendDisable();
            this.weapons[2].SendEnable();
            this.enabledWeapon = 2;
        }
    }

    private void Elevation() {
        this.normalrotationY += Input.GetAxis("Mouse Y") * Configuration.sensitivity * this.motor.sensimag;
        this.normalrotationY = Mathf.Clamp(this.normalrotationY, this.minimumY, this.maximumY);
        this.rotationY = this.normalrotationY + this.recoilrotationy;
    }

    private void RecoilControl() {
        if ((this.recoilrotationx == 0 && this.recoilrotationy == 0) || this.isRecoiling) {
            return;
        }

        if (this.recoilrotationy > 0F) {
            this.recoilrotationy -= this.recoilFixSpeed * Time.deltaTime;
        }

        if (this.recoilrotationy < 0f) {
            this.recoilrotationy = 0f;
        }

        if (this.recoilrotationx != 0) {
            this.recoilrotationx = Mathf.Lerp(this.recoilrotationx, 0, 0.1F);
        }
    }

    private void DispersionControl() {
        this.dispersionRate = Mathf.Clamp(this.dispersionRate - 3f * Time.deltaTime, 1, this.dispersionRate);
        this.desiredDispersion = this.dispersionRate * DispersionCorrection() * Screen.height * 0.001f;
    }

    private void SetTarget() {
        Ray ray = Camera.main.ScreenPointToRay(this.center);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000, this.mask)) {
            this.targetPos = ray.GetPoint(1000);
            return;
        }
        this.targetPos = hit.point;
        this.targetObject = hit.collider.gameObject;
        if (this.targetObject.layer == 10) {
            StopCoroutine("ShowEnemyName");
            StartCoroutine("ShowEnemyName");
        }
    }

    private IEnumerator ShowEnemyName() {
        UIManager.Instance.StatusUI.TargetingEnemyName = this.targetObject.GetComponentInParent<FriendOrEnemy>().playerName;
        yield return new WaitForSeconds(0.2f);
        UIManager.Instance.StatusUI.TargetingEnemyName = "";
    }

    public void HitMark() {
        StopCoroutine("HitMarkCoroutine");
        StartCoroutine("HitMarkCoroutine");
    }

    private IEnumerator HitMarkCoroutine() {
        foreach (Sight sight in this.sights) {
            sight.SetColor(Color.red);
        }

        yield return new WaitForSeconds(0.3f);
        foreach (Sight sight in this.sights) {
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
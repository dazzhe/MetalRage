using UnityEngine;

public class Mech : MonoBehaviour {
    [SerializeField]
    private GameObject playerCameraPrefab;

    public GameObject TargetObject { get; set; }
    public bool IsRecoiling { get; set; }
    public float RotationY { get; set; }
    public float BaseRotationY { get; set; }
    public Vector2 RecoilRotation { get; set; }
    public float DispersionRate { get; set; }
    public float Dispersion { get; set; }
    public PlayerCamera PlayerCamera { get; set; }

    public Vector3 Center { get => new Vector3(Screen.width / 2, Screen.height / 2, 0); }
    public float SensitivityScale => 1f; //this.weapons[this.selecedWeaponIndex].SensitivityScale;
    public bool HideEnemyName { get; set; } = false;

    private int selecedWeaponIndex = 0;  //0 = Main, 1 = Right, 2 = Left
    private RangeFloat elevationRange = new RangeFloat(-60f, 60f);
    private float recoilFixSpeed = 25f;

    private void Awake() {
        this.PlayerCamera = Instantiate(this.playerCameraPrefab).GetComponent<PlayerCamera>();
        this.PlayerCamera.Target = this.transform;
    }

    private void Update() {
        if (UIManager.Instance.MenuUI.ActiveWindowLevel == 0) {
            WeaponSelect();
            Elevation();
        }
        if (!this.HideEnemyName) {
            SetTarget();
        }
        SuppressRecoil();
    }

    private void WeaponSelect() {
        //if (Input.GetButtonDown("MainWeapon") && this.selecedWeaponIndex != 0) {
        //    this.weapons[this.selecedWeaponIndex].Unselect();
        //    this.weapons[0].Select();
        //    this.selecedWeaponIndex = 0;
        //}
        //if (Input.GetButtonDown("RightWeapon") && this.selecedWeaponIndex != 1) {
        //    this.weapons[this.selecedWeaponIndex].Unselect();
        //    this.weapons[1].Select();
        //    this.selecedWeaponIndex = 1;
        //}
        //if (Input.GetButtonDown("LeftWeapon") && this.selecedWeaponIndex != 2) {
        //    this.weapons[this.selecedWeaponIndex].Unselect();
        //    this.weapons[2].Select();
        //    this.selecedWeaponIndex = 2;
        //}
    }

    private void Elevation() {
        this.BaseRotationY += Input.GetAxis("Mouse Y") * Configuration.Sensitivity.GetFloat() * this.SensitivityScale;
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

    private void SetTarget() {
        Ray ray = Camera.main.ScreenPointToRay(this.Center);
        var layerMask = LayerMask.GetMask("Player");
        layerMask = ~layerMask;
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, layerMask)) {
            return;
        }
        this.TargetObject = hit.collider.gameObject;
        if (this.TargetObject.layer == LayerMask.NameToLayer("Enemy")) {
            UIManager.Instance.StatusUI.TargetingEnemyName = this.TargetObject.GetComponentInParent<FriendOrEnemy>().playerName;
        } else {
            UIManager.Instance.StatusUI.TargetingEnemyName = "";
        }
    }

    public void HitMark() {
        //this.weapons[this.selecedWeaponIndex].Crosshair.ShowHitMark(0.5f);
    }

    private void OnDestroy() {
        if (UIManager.Instance == null) {
            return;
        }
        UIManager.Instance.StatusUI.TargetingEnemyName = "";
    }
}

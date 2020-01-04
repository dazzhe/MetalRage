using UnityEngine;

public class Mech : MonoBehaviour {
    [SerializeField]
    private GameObject playerCameraPrefab = default;
    [SerializeField]
    private Transform cameraFollowTarget = default;
    [SerializeField]
    private MechStatus mechStatus = default;
    [SerializeField]
    private MechMotor mechMotor = default;
    [SerializeField]
    private RangeFloat elevationRange = new RangeFloat(-60f, 60f);

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
    public RangeFloat ElevationRange { get => this.elevationRange; set => this.elevationRange = value; }
    public Transform CameraFollowTarget { get => this.cameraFollowTarget; set => this.cameraFollowTarget = value; }

    private int selecedWeaponIndex = 0;  //0 = Main, 1 = Right, 2 = Left
    private float recoilFixSpeed = 25f;

    private void Awake() {
        this.PlayerCamera = Instantiate(this.playerCameraPrefab).GetComponent<PlayerCamera>();
        this.PlayerCamera.Target = this.CameraFollowTarget;
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

    private void SuppressRecoil() {
        if ((Mathf.Abs(this.RecoilRotation.x) < Mathf.Epsilon && Mathf.Abs(this.RecoilRotation.y) < Mathf.Epsilon) || this.IsRecoiling) {
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
}

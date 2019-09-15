using UnityEngine;

public class FollowingCamera : MonoBehaviour {
    [SerializeField]
    private Vector3 cameraOffset;
    [SerializeField]
    private Transform mainWeaponTransform;

    private Transform cameraTransform;
    private Vector3 cp;
    private WeaponControl weaponctrl;
    private UnitMotor motor;
    private float positionZ;
    private Vector3 defaultPosition;
    private Vector3 oldfd;
    private Vector3 position;
    private Vector3 desiredPosition;
    private Vector3 closeOffset = new Vector3(0f, 2.5f, -1f);
    private float offset = 0;
    private bool leaning;
    private int leanDirection;

    private void Start() {
        this.leaning = false;
        this.weaponctrl = GetComponent<WeaponControl>();
        this.motor = GetComponent<UnitMotor>();
        this.oldfd = this.mainWeaponTransform.forward;
    }

    private void OnEnable() {
        if (!this.cameraTransform && Camera.main) {
            this.cameraTransform = Camera.main.transform;
        }
        if (!this.cameraTransform) {
            this.enabled = false;
        }
    }
    
    private void LateUpdate() {
        Setting();
        Direction();
        Lean();
        if (!this.leaning) {
            Follow();
            ForwardPosition();
        } else {
            this.offset = Mathf.Lerp(this.offset, this.leanDirection * 5f, 0.2F);
            this.position = this.defaultPosition + this.transform.right * this.offset;
        }
        Vector3 closePosition = this.transform.position + this.transform.rotation * this.closeOffset;
        Vector3 closeToFar = this.position - closePosition;
        if (Physics.Raycast(closePosition, closeToFar, out RaycastHit hit, closeToFar.magnitude + 0.03f)) {
            this.desiredPosition = hit.point;
        } else {
            this.desiredPosition = this.position;
        }
        this.cameraTransform.position = this.desiredPosition;
    }

    private void Setting() {
        this.cp = this.transform.position;
        this.cp.y += 2.5f;
        this.defaultPosition = this.cameraOffset + this.cp + this.positionZ * this.mainWeaponTransform.forward;
    }

    private void Lean() {
        if (this.motor.MoveDirection.x == 0 && this.motor.MoveDirection.z == 0) {
            if (Input.GetButtonDown("right lean")) {
                AudioManager.Instance.PlayLeanSE();
                this.leaning = true;
                this.leanDirection = 1;
            }
            if (Input.GetButtonDown("left lean")) {
                AudioManager.Instance.PlayLeanSE();
                this.leaning = true;
                this.leanDirection = -1;
            }
        }
        if (!Input.GetButton("right lean") && !Input.GetButton("left lean") && this.leaning) {
            this.leaning = false;
            this.offset = 0;
        }
    }

    private void Direction() {
        this.cameraTransform.rotation = this.mainWeaponTransform.rotation;
        this.position = this.cp + Quaternion.FromToRotation(this.oldfd, this.mainWeaponTransform.forward) * (this.position - this.cp);
        this.oldfd = this.mainWeaponTransform.forward;
    }

    private void ForwardPosition() {
        this.positionZ = -5.5f + Mathf.Abs(this.weaponctrl.RotationY) / 15f;
    }

    private void Follow() {
        var localPosition = this.transform.InverseTransformDirection(this.position);
        var defaultLocalPosition = this.transform.InverseTransformDirection(this.defaultPosition);
        localPosition.x = Mathf.Lerp(localPosition.x, defaultLocalPosition.x, 7f * Time.deltaTime);
        localPosition.y = Mathf.Lerp(localPosition.y, defaultLocalPosition.y, 20f * Time.deltaTime);
        localPosition.z = Mathf.Lerp(localPosition.z, defaultLocalPosition.z, 20f * Time.deltaTime);
        this.position = this.transform.TransformDirection(localPosition);
        this.defaultPosition = this.transform.TransformDirection(defaultLocalPosition);
    }
}
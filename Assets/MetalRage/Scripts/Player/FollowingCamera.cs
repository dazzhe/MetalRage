using UnityEngine;

public enum CameraLeanType {
    Right,
    Left,
    None
}

public class FollowingCamera : MonoBehaviour {
    [SerializeField]
    private Vector3 cameraOffset;
    [SerializeField]
    private Transform target = default; // Set this as MainWeapon.
    [Min(0f)]
    [SerializeField]
    private float leanLength = 5f;

    private new Camera camera;

    private CameraLeanType currentLeanType = CameraLeanType.None;
    private Vector3 leanOffset;
    private Vector3 leanDirection;

    private Vector3 baseLocalPosition;
    private Vector3 localPosition;

    private bool IsTargetMoving {
        get {
            var motor = GetComponent<UnitMotor>();
            return motor.MoveDirection.x != 0 || motor.MoveDirection.z != 0 || motor.MoveDirection.y != 0;
        }
    }

    private Vector3 OffsetByPitch {
        get {
            var offsetLength = -5.5f + 4f * Mathf.Abs(GetComponent<WeaponControl>().RotationY) / 60f;
            return offsetLength * this.target.transform.forward;
        }
    }

    private bool IsRightLeanTriggered => Input.GetButtonDown("right lean");
    private bool IsLeftLeanTriggered => Input.GetButtonDown("left lean");

    private void Awake() {
        this.currentLeanType = CameraLeanType.None;
    }

    private void OnEnable() {
        if (this.camera == null && Camera.main != null) {
            this.camera = Camera.main;
        }
        if (this.camera == null) {
            this.enabled = false;
        }
    }
    
    private void LateUpdate() {
        this.baseLocalPosition = this.cameraOffset + this.target.position + this.OffsetByPitch;
        SetLeanType();
        if (this.currentLeanType == CameraLeanType.None) {
            Follow();
        } else {
            this.leanOffset = Vector3.Lerp(this.leanOffset, this.leanLength * this.leanDirection, 0.2f);
            this.localPosition = this.baseLocalPosition + this.leanOffset;
        }
        // Collision Check
        var cameraPositionFromTarget = this.localPosition - this.target.position;
        if (Physics.Raycast(this.target.position, cameraPositionFromTarget, out RaycastHit hit, cameraPositionFromTarget.magnitude + 0.03f)) {
            this.localPosition = hit.point;
        }
        this.camera.transform.rotation = this.target.rotation;
        this.camera.transform.position = this.target.TransformPoint(this.localPosition);
    }

    private void SetLeanType() {
        if (!this.IsTargetMoving) {
            if (this.IsRightLeanTriggered) {
                TriggerLean(CameraLeanType.Right);
            }
            if (this.IsLeftLeanTriggered) {
                TriggerLean(CameraLeanType.Left);
            }
        }
        var isLeanCanceled =
            this.currentLeanType == CameraLeanType.Left && !Input.GetButton("right lean") ||
            this.currentLeanType == CameraLeanType.Right && !Input.GetButton("left lean");
        if (isLeanCanceled) {
            TriggerLean(CameraLeanType.None);
        }
    }

    private void TriggerLean(CameraLeanType leanType) {
        switch (leanType) {
        case CameraLeanType.Left:
            AudioManager.Instance.PlayLeanSE();
            this.currentLeanType = CameraLeanType.Left;
            this.leanDirection = -this.transform.right;
            break;
        case CameraLeanType.Right:
            AudioManager.Instance.PlayLeanSE();
            this.currentLeanType = CameraLeanType.Right;
            this.leanDirection = this.transform.right;
            break;
        case CameraLeanType.None:
            this.currentLeanType = CameraLeanType.None;
            this.leanOffset = Vector3.zero;
            break;
        }
    }

    private void Follow() {
        this.localPosition.x = Mathf.Lerp(this.localPosition.x, this.baseLocalPosition.x, 7f * Time.deltaTime);
        this.localPosition.y = Mathf.Lerp(this.localPosition.y, this.baseLocalPosition.y, 20f * Time.deltaTime);
        this.localPosition.z = Mathf.Lerp(this.localPosition.z, this.baseLocalPosition.z, 20f * Time.deltaTime);
    }
}

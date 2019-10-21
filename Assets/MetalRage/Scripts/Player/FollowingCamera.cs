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
    private Vector3 basePosition;

    private bool IsTargetMoving {
        get {
            var motor = GetComponent<UnitMotor>();
            return motor.MoveDirection.x != 0 || motor.MoveDirection.z != 0 || motor.MoveDirection.y != 0;
        }
    }

    private Vector3 OffsetByPitch {
        get {
            var offsetLength = -5.5f + 4f * Mathf.Abs(GetComponent<Robot>().RotationY) / 60f;
            return offsetLength * Vector3.forward;
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
    
    private void Update() {
        this.basePosition = this.target.TransformPoint(this.cameraOffset + this.OffsetByPitch);
        this.basePosition = AvoidWallPenetration(this.basePosition);
        SetLeanType();
        UpdateByTargetRotation();
        if (this.currentLeanType == CameraLeanType.None) {
            Follow();
        } else {
            this.leanOffset = Vector3.Lerp(this.leanOffset, this.leanLength * this.leanDirection, 0.2f);
            this.camera.transform.position = this.basePosition + this.leanOffset;
        }
        this.camera.transform.position = AvoidWallPenetration(this.camera.transform.position);
    }

    // Converts position for avoiding penetration into walls.
    private Vector3 AvoidWallPenetration(Vector3 position) {
        var positionFromTarget = position - this.target.position;
        var layerMask = ~LayerMask.NameToLayer("Player");
        var isRayHit =
            Physics.Raycast(this.target.position, positionFromTarget,
            out RaycastHit hit, positionFromTarget.magnitude + 0.04f,
            layerMask, QueryTriggerInteraction.Ignore);
        return isRayHit
             ? hit.point - positionFromTarget.normalized * 0.03f
             : position;
    }

    private void UpdateByTargetRotation() {
        // Camera rotates around the target by the same amount as rotation of the target from the last frame.
        var rotationDiff = this.target.transform.rotation * Quaternion.Inverse(this.camera.transform.rotation);
        var localPosition = this.target.InverseTransformPoint(this.camera.transform.position);
        var updatedLocalPosition = rotationDiff * localPosition;
        this.camera.transform.position = this.target.TransformPoint(updatedLocalPosition);
        this.camera.transform.rotation = this.target.rotation;
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
        var cameraPosition = this.camera.transform.position;
        cameraPosition.x = Mathf.Lerp(cameraPosition.x, this.basePosition.x, 7f * Time.deltaTime);
        cameraPosition.y = Mathf.Lerp(cameraPosition.y, this.basePosition.y, 20f * Time.deltaTime);
        cameraPosition.z = Mathf.Lerp(cameraPosition.z, this.basePosition.z, 20f * Time.deltaTime);
        this.camera.transform.position = cameraPosition;
    }
}

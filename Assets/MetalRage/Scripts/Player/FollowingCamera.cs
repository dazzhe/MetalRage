using UnityEngine;

public enum CameraLeanMode {
    Right,
    Left,
    None
}

public class FollowingCamera : MonoBehaviour {
    [SerializeField]
    private Vector3 baseCameraOffset = new Vector3(0f, 0.5f, -5.5f);
    [SerializeField]
    private Transform target = default; // Set this as MainWeapon.
    [Min(0f)]
    [SerializeField]
    private float leanLength = 5f;

    private new Camera camera;
    private CameraLeanMode currentLeanMode = CameraLeanMode.None;
    private Vector3 currentLeanOffset;

    private bool IsTargetMoving {
        get {
            var motor = GetComponent<UnitMotor>();
            return motor.MoveDirection.x != 0 || motor.MoveDirection.z != 0 || motor.MoveDirection.y != 0;
        }
    }

    private Vector3 OffsetByPitch {
        get {
            var offsetLength =  Mathf.Abs(GetComponent<Robot>().RotationY) / 15f;
            return offsetLength * Vector3.forward;
        }
    }

    private bool IsRightLeanTriggered => Input.GetButtonDown("right lean");
    private bool IsLeftLeanTriggered => Input.GetButtonDown("left lean");

    private void Awake() {
        this.currentLeanMode = CameraLeanMode.None;
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
        var cameraOffset = this.baseCameraOffset + this.OffsetByPitch;
        SetLeanType();
        UpdateByTargetRotation();
        if (this.currentLeanMode == CameraLeanMode.None) {
            var desiredCameraPosition = AvoidWallPenetration(this.target.TransformPoint(cameraOffset));
            Follow(desiredCameraPosition);
        } else {
            var leanDirection = this.currentLeanMode == CameraLeanMode.Left ? Vector3.left : Vector3.right;
            var leanOffset = this.leanLength * leanDirection;
            this.currentLeanOffset = Vector3.Lerp(this.currentLeanOffset, leanOffset, 30f * Time.deltaTime);
            this.camera.transform.position = this.target.TransformPoint(cameraOffset + this.currentLeanOffset);
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
                TriggerLean(CameraLeanMode.Right);
            }
            if (this.IsLeftLeanTriggered) {
                TriggerLean(CameraLeanMode.Left);
            }
        }
        var isLeanCanceled =
            this.currentLeanMode == CameraLeanMode.Left && !Input.GetButton("right lean") ||
            this.currentLeanMode == CameraLeanMode.Right && !Input.GetButton("left lean");
        if (isLeanCanceled) {
            TriggerLean(CameraLeanMode.None);
        }
    }

    private void TriggerLean(CameraLeanMode leanType) {
        switch (leanType) {
        case CameraLeanMode.Left:
            AudioManager.Instance.PlayLeanSE();
            this.currentLeanMode = CameraLeanMode.Left;
            break;
        case CameraLeanMode.Right:
            AudioManager.Instance.PlayLeanSE();
            this.currentLeanMode = CameraLeanMode.Right;
            break;
        case CameraLeanMode.None:
            this.currentLeanMode = CameraLeanMode.None;
            break;
        }
        this.currentLeanOffset = Vector3.zero;
    }

    private void Follow(Vector3 destination) {
        var cameraPosition = this.camera.transform.position;
        cameraPosition.x = Mathf.Lerp(cameraPosition.x, destination.x, 7f * Time.deltaTime);
        cameraPosition.y = Mathf.Lerp(cameraPosition.y, destination.y, 20f * Time.deltaTime);
        cameraPosition.z = Mathf.Lerp(cameraPosition.z, destination.z, 20f * Time.deltaTime);
        this.camera.transform.position = cameraPosition;
    }

    private void FollowImmediate(Vector3 destination) {
        this.camera.transform.position = destination;
    }
}

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCameraComponent : MonoBehaviour {
    [SerializeField]
    private Vector3 baseCameraOffset = new Vector3(0f, 0.5f, -5.5f);
    [SerializeField]
    private Transform target = default; // Set this as MainWeapon.
    [Min(0f)]
    [SerializeField]
    private float leanLength = 5f;
    private float offsetDistanceByPitch = 4.7f;

    private new Camera camera;
    private CameraLeanState currentLeanMode = CameraLeanState.Off;
    private Vector3 currentLeanOffset;

    private bool IsTargetMoving {
        get {
            return false;
            //return motor.MoveDirection.x != 0 || motor.MoveDirection.z != 0 || motor.MoveDirection.y != 0;
        }
    }

    private Vector3 OffsetByPitch {
        get {
            var offsetLength = Mathf.Abs(Mathf.Sin(this.transform.localEulerAngles.x * Mathf.Deg2Rad)) * this.offsetDistanceByPitch;
            return offsetLength * Vector3.forward;
        }
    }

    private bool IsRightLeanTriggered => Input.GetButtonDown("right lean");
    private bool IsLeftLeanTriggered => Input.GetButtonDown("left lean");

    public Transform Target { get => this.target; set => this.target = value; }

    private void Awake() {
        this.currentLeanMode = CameraLeanState.Off;
        this.camera = GetComponent<Camera>();
    }

    private void LateUpdate() {
        var cameraOffset = this.baseCameraOffset + this.OffsetByPitch;
        SetLeanType();
        UpdateByTargetRotation();
        if (this.currentLeanMode == CameraLeanState.Off) {
            var desiredCameraPosition = AvoidWallPenetration(this.Target.TransformPoint(cameraOffset));
            Follow(desiredCameraPosition);
        } else {
            var leanDirection = this.currentLeanMode == CameraLeanState.Left ? Vector3.left : Vector3.right;
            var leanOffset = this.leanLength * leanDirection;
            this.currentLeanOffset = Vector3.Lerp(this.currentLeanOffset, leanOffset, 30f * Time.deltaTime);
            this.camera.transform.position = this.Target.TransformPoint(cameraOffset + this.currentLeanOffset);
        }
        // Double check wall penetration to avoid jittering.
        this.camera.transform.position = AvoidWallPenetration(this.camera.transform.position);
    }

    // Converts position for avoiding penetration into walls.
    private Vector3 AvoidWallPenetration(Vector3 position) {
        var positionFromTarget = position - this.Target.position;
        var layerMask = ~(1 << LayerMask.NameToLayer("Player"));
        var isRayHit =
            Physics.Raycast(this.Target.position, positionFromTarget,
            out RaycastHit hit, positionFromTarget.magnitude + 0.04f,
            layerMask, QueryTriggerInteraction.Ignore);
        if (isRayHit) {
            Debug.Log($"{hit.distance}, {hit.collider.gameObject.name}");
        }
        return isRayHit
             ? hit.point - positionFromTarget.normalized * 0.03f
             : position;
    }

    private void UpdateByTargetRotation() {
        // Camera rotates around the target by the same amount as rotation of the target from the last frame.
        var rotationDiff = this.Target.transform.rotation * Quaternion.Inverse(this.camera.transform.rotation);
        var localPosition = this.Target.InverseTransformPoint(this.camera.transform.position);
        var updatedLocalPosition = rotationDiff * localPosition;
        this.camera.transform.position = this.Target.TransformPoint(updatedLocalPosition);
        this.camera.transform.rotation = this.Target.rotation;
    }

    private void SetLeanType() {
        if (!this.IsTargetMoving) {
            if (this.IsRightLeanTriggered) {
                TriggerLean(CameraLeanState.Right);
            }
            if (this.IsLeftLeanTriggered) {
                TriggerLean(CameraLeanState.Left);
            }
        }
        var isLeanCanceled =
            this.currentLeanMode == CameraLeanState.Left && !Input.GetButton("right lean") ||
            this.currentLeanMode == CameraLeanState.Right && !Input.GetButton("left lean");
        if (isLeanCanceled) {
            TriggerLean(CameraLeanState.Off);
        }
    }

    private void TriggerLean(CameraLeanState leanType) {
        switch (leanType) {
            case CameraLeanState.Left:
                Game.SoundSystem.Play(SoundDefinition.LeanSound);
                this.currentLeanMode = CameraLeanState.Left;
                break;
            case CameraLeanState.Right:
                Game.SoundSystem.Play(SoundDefinition.LeanSound);
                this.currentLeanMode = CameraLeanState.Right;
                break;
            case CameraLeanState.Off:
                this.currentLeanMode = CameraLeanState.Off;
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

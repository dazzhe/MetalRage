using UnityEngine;

public class FollowingCamera : MonoBehaviour {
    public GameObject objc;
    public Transform cameraTransform;
    public Vector3 cp;

    private AudioSource lean;
    private WeaponControl weaponctrl;
    private UnitMotor motor;
    private float positionz;
    private Vector3 defaultposition;
    private Vector3 oldfd;
    private Vector3 position;
    private Vector3 desiredPosition;
    private Vector3 lposition;
    private Vector3 ldefaultposition;
    private Vector3 closeOffset = new Vector3(0f, 2.5f, -1f);
    private float offset = 0;
    private bool leaning;
    private int dir;

    private void Start() {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        this.lean = audioSources[1];
        this.leaning = false;
        this.weaponctrl = GetComponent<WeaponControl>();
        this.motor = GetComponent<UnitMotor>();
        this.oldfd = this.objc.transform.forward;
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
            Distance();
            //transform.position = cp + transform.right * positionx
            //	+ transform.forward * positionz;
        } else {
            this.offset = Mathf.Lerp(this.offset, this.dir * 5f, 0.2F);
            this.position = this.defaultposition + this.transform.right * this.offset;
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
        this.cp.y += 2.5F;
        this.defaultposition = this.cp + this.positionz * this.objc.transform.forward;
    }

    private void Lean() {
        if (this.motor.moveDirection.x == 0 && this.motor.moveDirection.z == 0) {
            if (Input.GetButtonDown("right lean")) {
                this.lean.PlayOneShot(this.lean.clip);
                this.leaning = true;
                this.dir = 1;
            }
            if (Input.GetButtonDown("left lean")) {
                this.lean.PlayOneShot(this.lean.clip);
                this.leaning = true;
                this.dir = -1;
            }
        }
        if (!Input.GetButton("right lean") && !Input.GetButton("left lean") && this.leaning) {
            this.leaning = false;
            this.offset = 0;
        }
    }

    private void Direction() {
        this.cameraTransform.rotation = this.objc.transform.rotation;
        this.position = this.cp + Quaternion.FromToRotation(this.oldfd, this.objc.transform.forward) * (this.position - this.cp);
        this.oldfd = this.objc.transform.forward;
    }

    private void Distance() {
        this.positionz = -5.5F + 4F * Mathf.Abs(this.weaponctrl.rotationY) / 60F;
    }

    private void Follow() {
        this.lposition = this.transform.InverseTransformDirection(this.position);
        this.ldefaultposition = this.transform.InverseTransformDirection(this.defaultposition);
        this.lposition.x = Mathf.Lerp(this.lposition.x, this.ldefaultposition.x, 7F * Time.deltaTime);
        this.lposition.y = Mathf.Lerp(this.lposition.y, this.ldefaultposition.y, 20F * Time.deltaTime);
        this.lposition.z = Mathf.Lerp(this.lposition.z, this.ldefaultposition.z, 20F * Time.deltaTime);
        this.position = this.transform.TransformDirection(this.lposition);
        this.defaultposition = this.transform.TransformDirection(this.ldefaultposition);
    }
}
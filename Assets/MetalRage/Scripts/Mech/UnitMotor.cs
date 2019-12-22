using System.Collections;
using UnityEngine;

public enum MechLocoState {
    Idle = 0,
    Walking = 1,
    Boosting = 2,
    Crouching = 3,
    Jumping = 4,
    Acceling = 5,
    Braking = 6
}

public class UnitMotor : MonoBehaviour {
    private CharacterController controller;
    private Mech robot;

    public bool canBoost = true;
    [SerializeField]
    private float boostSpeed = 40f;
    [SerializeField]
    private float jumpSpeed = 18f;
    [SerializeField]
    private float walkSpeed = 17f;
    [SerializeField]
    private float accelSpeed = 0.2f;
    [SerializeField]
    private Engine engine = default;

    private Vector3 lastPosition;
    private Vector3 velosity;
    private Vector3 groundNormal;
    private bool canJump = true;

    public MechLocoState locoState;
    public Vector3 MoveDirection;
    private Vector3 boostDirection;
    private Vector3 accelDirection;
    private Vector3 rawDirection;
    private float gravity = 40.0f;
    private Animator animator;

    //ジャンプしたときの水平方向の初速度.
    private float hJumpSpeed = 20f;
    public bool isGrounded = false;
    public int BoostGauge { get; private set; }
    public int MaxBoostGauge { get; private set; } = 100;

    private float boostCount;
    private float boostTime = 0.2F;

    private void Awake() {
        this.robot = GetComponent<Mech>();
        this.animator = GetComponent<Animator>();
    }

    private void Start() {
        StartCoroutine(BoostRegen());
        this.controller = GetComponent<CharacterController>();
        this.BoostGauge = this.MaxBoostGauge;
        this.lastPosition = this.transform.position;
    }

    private void Update() {
        Walk();
        Turn();
        Jump();
        if (this.canBoost) {
            BoostControl();
        }
        Crouch();
        MoveAndPushDown();
        LegDirection();
        if (!this.isGrounded) {
            this.MoveDirection.y -= this.gravity * Time.deltaTime;
        }
    }

    private void LegDirection() {
        var legYaw = 0f;
        var currentYaw = GetComponent<Animator>().GetFloat("LegOffsetYaw");
        if (this.locoState == MechLocoState.Walking) {
            var h = InputSystem.GetHorizontalMotion();
            var v = InputSystem.GetVerticalMotion();
            legYaw = h > 0 && v > 0 ? 45f :
                              h > 0 && v == 0 ? 90f :
                   (h == 0 && v > 0) || v < 0 ? 0f :
                               h < 0 && v > 0 ? -45f :
                              h < 0 && v == 0 ? -90f : currentYaw;
        }
        if (this.locoState == MechLocoState.Boosting) {
            legYaw = 0f;
        }
        GetComponent<Animator>().SetFloat("LegOffsetYaw", legYaw);
    }

    private void MoveAndPushDown() {
        this.velosity = (this.transform.position - this.lastPosition) / Time.deltaTime;
        this.lastPosition = this.transform.position;
        Vector3 currentMovementOffset = this.MoveDirection * Time.deltaTime;
        float pushDownOffset = Mathf.Max(this.controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
        if (this.isGrounded) {
            currentMovementOffset -= pushDownOffset * Vector3.up;
        }
        this.groundNormal = Vector3.zero;
        this.controller.Move(currentMovementOffset);
        if (this.isGrounded && !IsGroundedTest()) {
            SetIsGrounded(false);
            this.transform.position += pushDownOffset * Vector3.up;
        } else if (!this.isGrounded && IsGroundedTest()) {
            SetIsGrounded(true);
        }
    }

    private void Turn() {
        var yaw = this.transform.localEulerAngles.y + InputSystem.GetMouseX() * Configuration.Sensitivity.GetFloat() * this.robot.SensitivityScale;
        this.transform.localEulerAngles = new Vector3(0, yaw, 0);
        this.animator.SetFloat("AimOffsetPitch", this.robot.RotationY);
    }

    //入力情報から速度ベクトルを計算する.
    private void Walk() {
        if (this.isGrounded && (this.locoState == MechLocoState.Walking || this.locoState == MechLocoState.Idle)) {
            //まず-1<=x<=1,-1<=y<=1の範囲で動かすことでx,y方向それぞれの.
            //最大速度に対する相対値を計算している.
            switch (Mathf.RoundToInt(InputSystem.GetHorizontalMotion())) {
                case 1:
                    this.rawDirection.x += this.accelSpeed;
                    break;
                case -1:
                    this.rawDirection.x -= this.accelSpeed;
                    break;
                case 0:
                    this.rawDirection.x = Mathf.Lerp(this.rawDirection.x, 0, this.accelSpeed);
                    break;
            }
            switch (Mathf.RoundToInt(InputSystem.GetVerticalMotion())) {
                case 1:
                    this.rawDirection.z += this.accelSpeed;
                    break;
                case -1:
                    this.rawDirection.z -= this.accelSpeed;
                    break;
                case 0:
                    this.rawDirection.z = Mathf.Lerp(this.rawDirection.z, 0f, this.accelSpeed);
                    break;
            }
            this.rawDirection.x = Mathf.Clamp(this.rawDirection.x, -1, 1);
            this.rawDirection.z = Mathf.Clamp(this.rawDirection.z, -1, 1);
            //計算したベクトルの長さが0に近いときは0に丸め.
            //他は斜め方向でも最大値が1を超えないよう半径１の円の内部に変換している.
            if (this.rawDirection.magnitude <= 0.01f) {
                this.rawDirection = Vector2.zero;
                this.MoveDirection = this.rawDirection;
            } else {
                if (Mathf.Abs(this.rawDirection.x) >= Mathf.Abs(this.rawDirection.z)) {
                    this.MoveDirection
                        = Mathf.Abs(this.rawDirection.x) / this.rawDirection.magnitude * this.rawDirection;
                } else {
                    this.MoveDirection
                        = Mathf.Abs(this.rawDirection.z) / this.rawDirection.magnitude * this.rawDirection;
                }
            }
            this.MoveDirection = this.transform.TransformDirection(this.MoveDirection);
            this.MoveDirection *= this.walkSpeed;
            if (this.rawDirection.magnitude != 0) {
                this.locoState = MechLocoState.Walking;
            } else if (this.locoState == MechLocoState.Walking) {
                this.locoState = MechLocoState.Idle;
            }
        } else {
            if (this.locoState == MechLocoState.Walking) {
                this.locoState = 0;
            }

            if (this.rawDirection.magnitude != 0) {
                this.rawDirection = Vector3.zero;
            }
        }
        if (Mathf.Abs(this.MoveDirection.x) < 0.01) {
            this.MoveDirection.x = 0;
        }

        if (Mathf.Abs(this.MoveDirection.z) < 0.01) {
            this.MoveDirection.z = 0;
        }
        this.animator.SetFloat("WalkSpeed", this.MoveDirection.magnitude);
    }

    private void Crouch() {
        if (this.isGrounded && InputSystem.GetButton(MechCommandButton.Crouch) && this.locoState != MechLocoState.Boosting) {
            this.locoState = MechLocoState.Crouching;
            this.MoveDirection *= 0;
        } else if (this.locoState == MechLocoState.Crouching) {
            this.locoState = 0;
        }
    }

    private void Jump() {
        if (this.isGrounded) {
            if (InputSystem.GetButtonDown(MechCommandButton.Jump) && this.canJump && this.locoState != MechLocoState.Boosting) {
                this.isGrounded = false;
                StartCoroutine(JumpCoolDown());
                this.engine?.ShowJetFlame(0.4f, Vector3.forward);
                //移動速度が大きいほど高くジャンプさせる.
                this.MoveDirection
                    = this.transform.TransformDirection(new Vector3(InputSystem.GetHorizontalMotion() * this.hJumpSpeed,
                                                               this.jumpSpeed * (1f + 0.002f * this.velosity.magnitude),
                                                               InputSystem.GetVerticalMotion() * this.hJumpSpeed));
            } else if (this.locoState == MechLocoState.Jumping) {
                this.locoState = MechLocoState.Idle;
            }
        }
        if (!this.isGrounded && this.locoState != MechLocoState.Boosting) {
            this.locoState = MechLocoState.Jumping;
            float horizontalSpeed = new Vector2(this.MoveDirection.x, this.MoveDirection.z).magnitude;
            this.accelDirection = new Vector3(InputSystem.GetHorizontalMotion(), 0, InputSystem.GetVerticalMotion());

            if (horizontalSpeed > this.hJumpSpeed) {
                this.MoveDirection.x = this.MoveDirection.x * this.hJumpSpeed / horizontalSpeed;
                this.MoveDirection.z = this.MoveDirection.z * this.hJumpSpeed / horizontalSpeed;
            }
            this.accelDirection = this.transform.TransformDirection(this.accelDirection).normalized * 40F;
            this.MoveDirection += this.accelDirection * Time.deltaTime;
        }
    }

    private IEnumerator JumpCoolDown() {
        this.canJump = false;
        yield return new WaitForSeconds(1.5f);
        this.canJump = true;
    }

    private void BoostControl() {
        if (this.isGrounded && InputSystem.GetButtonDown(MechCommandButton.Boost)
            && (this.locoState == MechLocoState.Walking
                || this.locoState == MechLocoState.Idle
                || this.locoState == MechLocoState.Braking)
            && this.BoostGauge >= 28) {
            this.BoostGauge -= 28;
            float h = InputSystem.GetHorizontalMotion();
            float v = InputSystem.GetVerticalMotion();
            if (v != -1 && h != 0) {
                if (h == 1 && v == 1) {
                    this.boostDirection = this.transform.TransformDirection(new Vector3(1F, 0F, 1F).normalized);
                } else if (h == -1 && v == 1) {
                    this.boostDirection = this.transform.TransformDirection(new Vector3(-1F, 0F, 1F).normalized);
                } else if (h == 1 && this.rawDirection.z < -0.2) {
                    this.boostDirection = this.transform.TransformDirection(new Vector3(1F, 0F, -1F).normalized);
                } else if (h == -1 && this.rawDirection.z < -0.2) {
                    this.boostDirection = this.transform.TransformDirection(new Vector3(-1F, 0F, -1F).normalized);
                } else if (h == 1 && v == 0) {
                    this.boostDirection = this.transform.right;
                } else if (h == -1 && v == 0) {
                    this.boostDirection = -this.transform.right;
                } else {
                    this.boostDirection = this.transform.InverseTransformDirection(this.MoveDirection).normalized;
                }
            } else {
                this.boostDirection = this.transform.forward;
            }
            this.engine.ShowJetFlame(0.4f, Vector3.forward);
            StartCoroutine(BoostStart());
        }

        if (this.locoState == MechLocoState.Boosting) {
            Boost();
        }

        if (this.locoState == MechLocoState.Braking) {
            BoostBrake();
        }
    }

    private IEnumerator BoostStart() {
        // Jump and squat inputs are accepted only if CharacterState is "Acceling".
        this.MoveDirection = this.boostDirection * this.boostSpeed;
        this.locoState = MechLocoState.Acceling;
        yield return new WaitForSeconds(0.05f);
        if (this.locoState == MechLocoState.Acceling) {
            this.locoState = MechLocoState.Boosting;
            yield return new WaitForSeconds(this.boostTime);
            this.locoState = MechLocoState.Braking;
        }
    }

    private void Boost() {
        // Stop movement if the unit collides to a wall.
        if ((this.controller.collisionFlags & CollisionFlags.Sides) != 0) {
            this.MoveDirection = Vector3.zero;
        }
    }

    private void BoostBrake() {
        this.MoveDirection = Vector3.Lerp(this.MoveDirection, Vector3.zero, 6f * Time.deltaTime);
        if (this.MoveDirection.magnitude <= this.walkSpeed) {
            this.locoState = MechLocoState.Walking;
        }
    }

    private bool IsGroundedTest() {
        return (this.groundNormal.y > 0.01);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.normal.y > 0 && hit.normal.y > this.groundNormal.y && hit.moveDirection.y < 0) {
            this.groundNormal = hit.normal;
        }
    }

    private IEnumerator BoostRegen() {
        while (true) {
            if (this.BoostGauge != 100) {
                this.BoostGauge += 2;
                if (this.BoostGauge > 100) {
                    this.BoostGauge = 100;
                }
            }
            yield return new WaitForSeconds(0.15f);
        }
    }

    private void SetIsGrounded(bool value) {
        this.isGrounded = value;
        this.animator.SetBool("IsGrounded", value);
    }
}

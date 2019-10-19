using System.Collections;
using UnityEngine;

public enum RobotState {
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

    [System.NonSerialized]
    public byte inputState = 0;
    [System.NonSerialized]
    public Vector2 inputMoveDirection;
    private bool inputJump;
    private bool inputBoost;
    private bool inputSquat;

    private Vector3 lastPosition;
    private Vector3 velosity;
    private Vector3 groundNormal;
    private bool canJump = true;

    [System.NonSerialized]
    public RobotState characterState;
    public Vector3 MoveDirection;
    private Vector3 boostDirection;
    private Vector3 accelDirection;
    private Vector3 rawDirection;
    private float gravity = 40.0f;

    //ジャンプしたときの水平方向の初速度.
    private float hJumpSpeed = 20f;
    [System.NonSerialized]
    public float yaw = 0f;
    public bool isGrounded = false;
    [System.NonSerialized]
    public int boostgauge;
    [System.NonSerialized]
    public int boostmax = 100;

    private float boostCount;
    private float boostTime = 0.2F;

    private void Start() {
        StartCoroutine(BoostRegen());
        this.controller = GetComponent<CharacterController>();
        this.boostgauge = this.boostmax;
        this.lastPosition = this.transform.position;
    }

    private void LateUpdate() {
        ApplyInputState();
        Walk();
        Turn();
        Jump();
        if (this.canBoost) {
            BoostControl();
        }
        Crouch();
        MoveAndPushDown();
        if (!this.isGrounded) {
            this.MoveDirection.y -= this.gravity * Time.deltaTime;
        }
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
            this.isGrounded = false;
            this.transform.position += pushDownOffset * Vector3.up;
        } else if (!this.isGrounded && IsGroundedTest()) {
            this.isGrounded = true;
        }
    }

    // Decode and apply inputState.
    private void ApplyInputState() {
        if ((64 & this.inputState) != 0) {
            this.inputMoveDirection.x = 1;
            if ((128 & this.inputState) != 0) {
                this.inputMoveDirection.x = -this.inputMoveDirection.x;
            }
        } else {
            this.inputMoveDirection.x = 0;
        }
        if ((16 & this.inputState) != 0) {
            this.inputMoveDirection.y = 1;
            if ((32 & this.inputState) != 0) {
                this.inputMoveDirection.y = -this.inputMoveDirection.y;
            }
        } else {
            this.inputMoveDirection.y = 0;
        }
        this.inputJump = (8 & this.inputState) != 0;
        this.inputBoost = (4 & this.inputState) != 0;
        this.inputSquat = (2 & this.inputState) != 0;
    }
    
    private void Turn() {
        this.transform.localEulerAngles = new Vector3(0, this.yaw, 0);
    }

    //入力情報から速度ベクトルを計算する.
    private void Walk() {
        if (this.isGrounded && (this.characterState == RobotState.Walking || this.characterState == RobotState.Idle)) {
            //まず-1<=x<=1,-1<=y<=1の範囲で動かすことでx,y方向それぞれの.
            //最大速度に対する相対値を計算している.
            switch (Mathf.RoundToInt(this.inputMoveDirection.x)) {
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
            switch (Mathf.RoundToInt(this.inputMoveDirection.y)) {
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
                this.characterState = RobotState.Walking;
            } else if (this.characterState == RobotState.Walking) {
                this.characterState = RobotState.Idle;
            }
        } else {
            if (this.characterState == RobotState.Walking) {
                this.characterState = 0;
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
    }

    private void Crouch() {
        if (this.isGrounded && this.inputSquat && this.characterState != RobotState.Boosting) {
            this.characterState = RobotState.Crouching;
            this.MoveDirection *= 0;
        } else if (this.characterState == RobotState.Crouching) {
            this.characterState = 0;
        }
    }

    private void Jump() {
        if (this.isGrounded) {
            if (this.inputJump && this.canJump && this.characterState != RobotState.Boosting) {
                this.isGrounded = false;
                StartCoroutine(JumpCoolDown());
                this.engine?.ShowJetFlame(0.4f, Vector3.forward);
                //移動速度が大きいほど高くジャンプさせる.
                this.MoveDirection
                    = this.transform.TransformDirection(new Vector3(this.inputMoveDirection.x * this.hJumpSpeed,
                                                               this.jumpSpeed * (1f + 0.002f * this.velosity.magnitude),
                                                               this.inputMoveDirection.y * this.hJumpSpeed));
            } else if (this.characterState == RobotState.Jumping) {
                this.characterState = RobotState.Idle;
            }
        }
        if (!this.isGrounded && this.characterState != RobotState.Boosting) {
            this.characterState = RobotState.Jumping;
            float horizontalSpeed = new Vector2(this.MoveDirection.x, this.MoveDirection.z).magnitude;
            this.accelDirection = new Vector3(this.inputMoveDirection.x, 0, this.inputMoveDirection.y);

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
        if (this.isGrounded && this.inputBoost
            && (this.characterState == RobotState.Walking
                || this.characterState == RobotState.Idle
                || this.characterState == RobotState.Braking)
            && this.boostgauge >= 28) {
            this.boostgauge -= 28;
            float h = this.inputMoveDirection.x;
            float v = this.inputMoveDirection.y;
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

        if (this.characterState == RobotState.Boosting) {
            Boost();
        }

        if (this.characterState == RobotState.Braking) {
            BoostBrake();
        }
    }

    private IEnumerator BoostStart() {
        // Jump and squat inputs are accepted only if CharacterState is "Acceling".
        this.MoveDirection = this.boostDirection * this.boostSpeed;
        this.characterState = RobotState.Acceling;
        yield return new WaitForSeconds(0.05f);
        if (this.characterState == RobotState.Acceling) {
            this.characterState = RobotState.Boosting;
            yield return new WaitForSeconds(this.boostTime);
            this.characterState = RobotState.Braking;
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
            this.characterState = RobotState.Walking;
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
            if (this.boostgauge != 100) {
                this.boostgauge += 2;
                if (this.boostgauge > 100) {
                    this.boostgauge = 100;
                }
            }
            yield return new WaitForSeconds(0.15f);
        }
    }
}

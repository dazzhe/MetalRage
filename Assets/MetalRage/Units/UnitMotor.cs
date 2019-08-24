using System.Collections;
using UnityEngine;

public class UnitMotor : MonoBehaviour {
    private CharacterController controller;
    private AudioSource boost;

    public bool canBoost = true;
    [SerializeField]
    private float boostspeed = 40F;
    [SerializeField]
    private float jumpSpeed = 18F;
    [SerializeField]
    private float walkspeed = 17F;
    [SerializeField]
    private float accspeed = 0.2f;

    [System.NonSerialized]
    public byte inputState = 0;
    [System.NonSerialized]
    public Vector2 inputMoveDirection;
    private bool inputJump;
    private bool inputBoost;
    private bool inputSquat;

    [System.NonSerialized]
    public float sensimag = 1f;

    private Vector3 lastPosition;
    private Vector3 velosity;
    private Vector3 groundNormal;
    private bool canJump = true;
    public enum CharacterState {
        Idle = 0,
        Walking = 1,
        Boosting = 2,
        Squatting = 3,
        Jumping = 4,
        Acceling = 5,
        Braking = 6
    }
    [System.NonSerialized]
    public CharacterState _characterState;
    [System.NonSerialized]
    public Vector3 moveDirection =
        Vector3.zero;
    private Vector3 boostDirection =
        Vector3.zero;
    private Vector3 accelDirection =
        Vector3.zero;
    private Vector3 rawDirection =
        Vector3.zero;
    private float gravity = 40.0F;

    //ジャンプしたときの水平方向の初速度.
    private float hJumpSpeed = 20f;
    [System.NonSerialized]
    public float rotationX = 0F;
    private bool grounded = false;
    [System.NonSerialized]
    public int boosttype;
    [System.NonSerialized]
    public int boostgauge;
    [System.NonSerialized]
    public int boostmax = 100;

    private float boostCount;
    private float boostTime = 0.2F;

    private void Start() {
        StartCoroutine("BoostRegen");
        this.controller = GetComponent<CharacterController>();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        this.boost = audioSources[0];
        this.boostgauge = this.boostmax;
        this.lastPosition = this.transform.position;
    }

    //入力情報を受け取った後で処理を実行したほうが良いのでLateUpdateを使う.
    private void LateUpdate() {
        ApplyInputState();
        Walk();
        Turn();
        Jump();
        if (this.canBoost) {
            BoostControl();
        }

        Squat();
        MoveAndPushDown();
        if (!this.grounded) {
            this.moveDirection.y -= this.gravity * Time.deltaTime;
        }
    }

    //緩やかな坂を下るときに機体が浮いてしまう場合機体位置を地面方向に修正する.
    //坂が急すぎるときはgroundedをfalseにする.
    private void MoveAndPushDown() {
        this.velosity = (this.transform.position - this.lastPosition) / Time.deltaTime;
        this.lastPosition = this.transform.position;
        Vector3 currentMovementOffset = this.moveDirection * Time.deltaTime;

        float pushDownOffset = Mathf.Max(this.controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
        if (this.grounded) {
            currentMovementOffset -= pushDownOffset * Vector3.up;
        }

        this.groundNormal = Vector3.zero;
        this.controller.Move(currentMovementOffset);
        if (this.grounded && !IsGroundedTest()) {
            this.grounded = false;
            this.transform.position += pushDownOffset * Vector3.up;
        } else if (!this.grounded && IsGroundedTest()) {
            this.grounded = true;
        }
    }

    //ビット演算を使って入力情報をbool変数に変えている.
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

    //機体全体を水平方向に回転させる.
    private void Turn() {
        this.transform.localEulerAngles = new Vector3(0, this.rotationX, 0);
    }

    //入力情報から速度ベクトルを計算する.
    private void Walk() {
        if (this.grounded && (this._characterState == CharacterState.Walking || this._characterState == CharacterState.Idle)) {
            //まず-1<=x<=1,-1<=y<=1の範囲で動かすことでx,y方向それぞれの.
            //最大速度に対する相対値を計算している.
            switch (Mathf.RoundToInt(this.inputMoveDirection.x)) {
                case 1:
                    this.rawDirection.x += this.accspeed;
                    break;
                case -1:
                    this.rawDirection.x -= this.accspeed;
                    break;
                case 0:
                    this.rawDirection.x = Mathf.Lerp(this.rawDirection.x, 0, this.accspeed);
                    break;
            }
            switch (Mathf.RoundToInt(this.inputMoveDirection.y)) {
                case 1:
                    this.rawDirection.z += this.accspeed;
                    break;
                case -1:
                    this.rawDirection.z -= this.accspeed;
                    break;
                case 0:
                    this.rawDirection.z = Mathf.Lerp(this.rawDirection.z, 0f, this.accspeed);
                    break;
            }
            this.rawDirection.x = Mathf.Clamp(this.rawDirection.x, -1, 1);
            this.rawDirection.z = Mathf.Clamp(this.rawDirection.z, -1, 1);
            //計算したベクトルの長さが0に近いときは0に丸め.
            //他は斜め方向でも最大値が1を超えないよう半径１の円の内部に変換している.
            if (this.rawDirection.magnitude <= 0.01f) {
                this.rawDirection = Vector2.zero;
                this.moveDirection = this.rawDirection;
            } else {
                if (Mathf.Abs(this.rawDirection.x) >= Mathf.Abs(this.rawDirection.z)) {
                    this.moveDirection
                        = Mathf.Abs(this.rawDirection.x) / this.rawDirection.magnitude * this.rawDirection;
                } else {
                    this.moveDirection
                        = Mathf.Abs(this.rawDirection.z) / this.rawDirection.magnitude * this.rawDirection;
                }
            }
            this.moveDirection = this.transform.TransformDirection(this.moveDirection);
            this.moveDirection *= this.walkspeed;
            if (this.rawDirection.magnitude != 0) {
                this._characterState = CharacterState.Walking;
            } else if (this._characterState == CharacterState.Walking) {
                this._characterState = CharacterState.Idle;
            }
        } else {
            if (this._characterState == CharacterState.Walking) {
                this._characterState = 0;
            }

            if (this.rawDirection.magnitude != 0) {
                this.rawDirection = Vector3.zero;
            }
        }
        if (Mathf.Abs(this.moveDirection.x) < 0.01) {
            this.moveDirection.x = 0;
        }

        if (Mathf.Abs(this.moveDirection.z) < 0.01) {
            this.moveDirection.z = 0;
        }
    }

    private void Squat() {
        if (this.grounded && this.inputSquat && this._characterState != CharacterState.Boosting) {
            this._characterState = CharacterState.Squatting;
            this.moveDirection *= 0;
        } else if (this._characterState == CharacterState.Squatting) {
            this._characterState = 0;
        }
    }

    private void Jump() {
        if (this.grounded) {
            if (this.inputJump && this.canJump && this._characterState != CharacterState.Boosting) {
                this.grounded = false;
                StartCoroutine("JumpCoolDown");
                this.boost.PlayOneShot(this.boost.clip);
                //移動速度が大きいほど高くジャンプさせる.
                this.moveDirection
                    = this.transform.TransformDirection(new Vector3(this.inputMoveDirection.x * this.hJumpSpeed,
                                                               this.jumpSpeed * (1f + 0.002f * this.velosity.magnitude),
                                                               this.inputMoveDirection.y * this.hJumpSpeed));
            } else if (this._characterState == CharacterState.Jumping) {
                this._characterState = CharacterState.Idle;
            }
        }
        if (!this.grounded && this._characterState != CharacterState.Boosting) {
            this._characterState = CharacterState.Jumping;
            float horizontalSpeed = new Vector2(this.moveDirection.x, this.moveDirection.z).magnitude;
            this.accelDirection = new Vector3(this.inputMoveDirection.x, 0, this.inputMoveDirection.y);

            if (horizontalSpeed > this.hJumpSpeed) {
                this.moveDirection.x = this.moveDirection.x * this.hJumpSpeed / horizontalSpeed;
                this.moveDirection.z = this.moveDirection.z * this.hJumpSpeed / horizontalSpeed;
            }
            this.accelDirection = this.transform.TransformDirection(this.accelDirection).normalized * 40F;
            this.moveDirection += this.accelDirection * Time.deltaTime;
        }
    }

    private IEnumerator JumpCoolDown() {
        this.canJump = false;
        yield return new WaitForSeconds(1.5f);
        this.canJump = true;
    }

    private void BoostControl() {
        if (this.grounded && this.inputBoost
            && (this._characterState == CharacterState.Walking
                || this._characterState == CharacterState.Idle
                || this._characterState == CharacterState.Braking)
            && this.boostgauge >= 28) {
            this.boost.PlayOneShot(this.boost.clip);
            this.boostgauge -= 28;
            float h = this.inputMoveDirection.x;
            float v = this.inputMoveDirection.y;
            //boosttypeを設定しているのはアニメーションのため(未実装).
            if (this.inputMoveDirection.y != -1 && h != 0) {
                this.boostDirection = this.transform.InverseTransformDirection(this.moveDirection.normalized);

                if (h == 1 && v == 1) {
                    this.boostDirection = this.transform.TransformDirection(new Vector3(1F, 0F, 1F).normalized);
                    this.boosttype = 0;
                } else if (h == -1 && v == 1) {
                    this.boostDirection = this.transform.TransformDirection(new Vector3(-1F, 0F, 1F).normalized);
                    this.boosttype = 2;
                } else if (h == 1 && this.rawDirection.z < -0.2) {
                    this.boostDirection = this.transform.TransformDirection(new Vector3(1F, 0F, -1F).normalized);
                    this.boosttype = 0;
                } else if (h == -1 && this.rawDirection.z < -0.2) {
                    this.boostDirection = this.transform.TransformDirection(new Vector3(-1F, 0F, -1F).normalized);
                    this.boosttype = 2;
                } else if (h == 1 && v == 0) {
                    this.boostDirection = this.transform.right;
                    this.boosttype = 0;
                } else if (h == -1 && v == 0) {
                    this.boostDirection = -this.transform.right;
                    this.boosttype = 2;
                }
            } else {
                this.boostDirection = this.transform.forward;
                this.boosttype = 1;
            }
            StartCoroutine("BoostStart");
        }

        if (this._characterState == CharacterState.Boosting) {
            Boost();
        }

        if (this._characterState == CharacterState.Braking) {
            BoostBrake();
        }
    }

    private IEnumerator BoostStart() {
        //Acceling中はジャンプとしゃがみを受け付ける.
        this.moveDirection = this.boostDirection * this.boostspeed;
        this._characterState = CharacterState.Acceling;
        yield return new WaitForSeconds(0.05f);
        if (this._characterState == CharacterState.Acceling) {
            this._characterState = CharacterState.Boosting;
            yield return new WaitForSeconds(this.boostTime);
            this._characterState = CharacterState.Braking;
        }
    }

    private void Boost() {
        //衝突した場合停止させる.
        if ((this.controller.collisionFlags & CollisionFlags.Sides) != 0) {
            this.moveDirection = Vector3.zero;
        }
    }

    private void BoostBrake() {
        this.moveDirection = Vector3.Lerp(this.moveDirection, Vector3.zero, 6f * Time.deltaTime);
        if (this.moveDirection.magnitude <= this.walkspeed) {
            this._characterState = CharacterState.Walking;
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

    //private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal) {
    //	Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
    ////	return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
    //}

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

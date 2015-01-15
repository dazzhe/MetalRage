using UnityEngine;
using System.Collections;
//input情報を元に機体を動かす.
public class UnitMotor : MonoBehaviour {
	CharacterController controller;
	private AudioSource boost;

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
	public Vector3 velosity;

	public float horizontalSpeed;
	private Vector3 groundNormal;
	public bool canBoost = true;
	private bool canJump = true;

	public enum CharacterState{
		Idle = 0,
		Walking = 1,
		Boosting = 2,
		Squatting = 3,
		Jumping = 4,
		Acceling = 5,
		Braking = 6
	}
	public CharacterState _characterState;
	public Vector3 moveDirection = 
		Vector3.zero;
	private Vector3 boostDirection = 
		Vector3.zero;
	private Vector3 accelDirection = 
		Vector3.zero;
	public Vector3 rawDirection =
		Vector3.zero;

	public static float gravity = 40.0F;
	
	public static float walkspeed = 17F;
	public static float accspeed = 0.2f;
	private float hspeed;
	private float vspeed;
	
	public static float boostspeed = 40F;
	public static float jumpSpeed = 18F;
	private float hJumpSpeed = 20f;
	public float rotationX = 0F;
	public bool grounded;
	
	public int boosttype;
	public int boostgauge;
	public int boostmax = 100;
	
	public float boostCount;
	public static float boostTime = 0.2F;

	// Use this for initialization
	void Start () {
		StartCoroutine("BoostRegen");
		controller = GetComponent<CharacterController>();
		AudioSource[] audioSources = GetComponents<AudioSource>();
		boost = audioSources[0];
		boostgauge = boostmax;
		grounded = controller.isGrounded;
		lastPosition = transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		ApplyInputState();
		Walk ();
		Turn ();
		Jump ();
		if (canBoost)
			BoostControl();
		Squat ();
		MoveAndPushDown ();
		ApplyMove();
	}
	//緩やかな坂を下るときに機体が浮いてしまう場合機体位置を地面方向に修正する.
	//坂が急すぎるときはgroundedをfalseにする.
	void MoveAndPushDown(){
		velosity = (transform.position - lastPosition) / Time.deltaTime;
		lastPosition = transform.position;
		Vector3 currentMovementOffset = moveDirection * Time.deltaTime;
		
		float pushDownOffset = Mathf.Max(controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
		if(grounded)
			currentMovementOffset -= pushDownOffset * Vector3.up;
		
		groundNormal = Vector3.zero;
		controller.Move(currentMovementOffset);
		if(grounded && !IsGroundedTest()){
			grounded = false;
			transform.position += pushDownOffset * Vector3.up;
		}
		else if(!grounded && IsGroundedTest()){
			grounded = true;
		}
	}

	//ビット演算を使って入力情報をbool変数に変えている.
	void ApplyInputState(){
		if ((64 & inputState) != 0){
			inputMoveDirection.x = 1;
			if ((128 & inputState) != 0)
				inputMoveDirection.x = -inputMoveDirection.x;
		} else
			inputMoveDirection.x = 0;
		if ((16 & inputState) != 0){
			inputMoveDirection.y = 1;
			if ((32 & inputState) != 0)
				inputMoveDirection.y = -inputMoveDirection.y;
		} else 
			inputMoveDirection.y = 0;
		inputJump = (8 & inputState) != 0;
		inputBoost = (4 & inputState) != 0;
		inputSquat = (2 & inputState) != 0;
	}

	void Turn(){
		transform.localEulerAngles = new Vector3(0, rotationX, 0);
	}

	//入力情報から速度ベクトルを計算する.
	void Walk(){
		if (grounded && (_characterState == CharacterState.Walking || _characterState == CharacterState.Idle)){
			//まず-1<=x<=1,-1<=y<=1の範囲で動かすことでx,y方向それぞれの.
			//最大速度に対する相対値を計算している.
			switch (Mathf.RoundToInt(inputMoveDirection.x)){
			case 1:
				rawDirection.x += accspeed;
				break;
			case -1:
				rawDirection.x -= accspeed;
				break;
			case 0:
				rawDirection.x = Mathf.Lerp(rawDirection.x, 0, accspeed);
				break;
			}
			switch (Mathf.RoundToInt(inputMoveDirection.y)){
			case 1:
				rawDirection.z += accspeed;
				break;
			case -1:
				rawDirection.z -= accspeed;
				break;
			case 0:
				rawDirection.z = Mathf.Lerp(rawDirection.z, 0f, accspeed);
				break;
			}
			rawDirection.x =Mathf.Clamp (rawDirection.x,-1,1);
			rawDirection.z =Mathf.Clamp (rawDirection.z,-1,1);
			//計算したベクトルの長さが0に近いときは0に丸め.
			//他は斜め方向でも最大値が1を超えないよう半径１の円の内部に変換している.
			if(rawDirection.magnitude <= 0.01f){
				rawDirection = Vector2.zero;
				moveDirection = rawDirection;
			}
			else {
				if (Mathf.Abs(rawDirection.x) >= Mathf.Abs(rawDirection.z))
					moveDirection 
						= Mathf.Abs(rawDirection.x) / rawDirection.magnitude * rawDirection;
				else
					moveDirection
						= Mathf.Abs (rawDirection.z) / rawDirection.magnitude * rawDirection;
			}
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= walkspeed;
			if (rawDirection.magnitude != 0)
				_characterState = CharacterState.Walking;
			else if (_characterState == CharacterState.Walking)
				_characterState = 0;
		}
		else{
			if (_characterState == CharacterState.Walking)
				_characterState = 0;
			if (rawDirection.magnitude != 0)
				rawDirection = Vector3.zero;
		}
		if (Mathf.Abs(moveDirection.x) < 0.01)
			moveDirection.x = 0;
		if (Mathf.Abs(moveDirection.z) < 0.01)
			moveDirection.z = 0;
	}
	
	void Squat(){
		if (grounded && inputSquat && _characterState != CharacterState.Boosting){
			_characterState = CharacterState.Squatting;
			moveDirection *= 0;
		} else if (_characterState == CharacterState.Squatting)
			_characterState = 0;
	}
	
	void Jump(){
		if (grounded) {
			if (inputJump && canJump && _characterState != CharacterState.Boosting){
				grounded = false;
				StartCoroutine("JumpCoolDown");
				boost.PlayOneShot(boost.clip);
				moveDirection
					= transform.TransformDirection(new Vector3(inputMoveDirection.x * hJumpSpeed,
					                                           jumpSpeed * (1f + 0.002f * velosity.magnitude),
					                                           inputMoveDirection.y * hJumpSpeed));
			} else if (_characterState == CharacterState.Jumping)
				_characterState = 0;
		}
		if (!grounded && _characterState != CharacterState.Boosting){
			_characterState = CharacterState.Jumping;
			horizontalSpeed = new Vector2(moveDirection.x, moveDirection.z).magnitude;
			accelDirection = new Vector3(inputMoveDirection.x, 0, inputMoveDirection.y);

			if (horizontalSpeed > hJumpSpeed){
				moveDirection.x = moveDirection.x * hJumpSpeed / horizontalSpeed;
				moveDirection.z = moveDirection.z * hJumpSpeed / horizontalSpeed;
			}
			accelDirection = transform.TransformDirection(accelDirection).normalized * 40F;
			moveDirection += accelDirection * Time.deltaTime;
		}
	}

	IEnumerator JumpCoolDown(){
		canJump = false;
		yield return new WaitForSeconds(1.5f);
		canJump = true;
	}
	
	void BoostControl(){
		if (grounded && inputBoost
		    && (_characterState == CharacterState.Walking || _characterState == CharacterState.Idle || _characterState == CharacterState.Braking)
		    && boostgauge >= 28) {
			boost.PlayOneShot (boost.clip);
			boostgauge -= 28;
			float h = inputMoveDirection.x;
			float v = inputMoveDirection.y;
			if (inputMoveDirection.y != -1 && h != 0) {
				boostDirection = transform.InverseTransformDirection(moveDirection.normalized);
				
				if (h == 1 && v == 1){
					boostDirection = transform.TransformDirection(new Vector3(1F, 0F, 1F).normalized);
					boosttype = 0;
				}
				else if (h == -1 && v == 1){
					boostDirection = transform.TransformDirection(new Vector3(-1F, 0F, 1F).normalized);
					boosttype = 2;
				}
				else if (h == 1 && rawDirection.y < 0){
					boostDirection = transform.TransformDirection(new Vector3(1F, 0F, -1F).normalized);
					boosttype = 0;
				}
				else if (h == -1 && rawDirection.y < 0){
					boostDirection = transform.TransformDirection(new Vector3(-1F, 0F, -1F).normalized);
					boosttype = 2;
				}
				else if (h == 1 && v == 0){
					boostDirection = transform.right;
					boosttype = 0;
				}
				else if (h == -1 && v == 0){
					boostDirection = -transform.right;
					boosttype = 2;
				}
			} else {
				boostDirection = transform.forward;
				boosttype = 1;
			}
			StartCoroutine("BoostStart");
		}
		
		if (_characterState == CharacterState.Boosting)
			Boost ();
		if (_characterState == CharacterState.Braking)
			BoostBrake();
	}
	
	IEnumerator BoostStart(){
		moveDirection = boostDirection * boostspeed;
		_characterState = CharacterState.Acceling;
		yield return new WaitForSeconds(0.05f);
		if (_characterState == CharacterState.Acceling){
			_characterState = CharacterState.Boosting;
			yield return new WaitForSeconds(boostTime);
			_characterState = CharacterState.Braking;
		}
	}

	void Boost(){
		if ((controller.collisionFlags & CollisionFlags.Sides) != 0)
			moveDirection = Vector3.zero;
	}
	
	void BoostBrake(){
		moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, 2f * Time.deltaTime);
		if (moveDirection.magnitude <= walkspeed)
			_characterState = CharacterState.Walking;
	}
	
	void ApplyMove(){
		if (!grounded)
			moveDirection.y -= gravity * Time.deltaTime;
		//controller.Move(moveDirection * 
		 //               Time.deltaTime);
	}

	private bool IsGroundedTest(){
		return (groundNormal.y > 0.01);
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if(hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0)
			this.groundNormal = hit.normal;
	}
	
	//private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal) {
	//	Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
	////	return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
	//}
	
	IEnumerator BoostRegen(){
		while(true){
			if (boostgauge != 100){
				boostgauge += 2;
				if (boostgauge > 100)
					boostgauge = 100;
			}
			yield return new WaitForSeconds(0.15f);
		}
	}
}

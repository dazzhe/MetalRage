using UnityEngine;

public class LegDirection : MonoBehaviour {
    [SerializeField]
    private GameObject unit;
    private UnitMotor motor;
    private float h;
    private float v;
    private static Vector3 rf =
        new Vector3(1, 0, 1);
    private static Vector3 lf =
        new Vector3(-1, 0, 1);

    private void Start() {
        this.motor = this.unit.GetComponent<UnitMotor>();
    }

    private void Update() {
        if (this.motor.characterState == UnitMotor.CharacterState.Walking) {
            this.h = this.motor.inputMoveDirection.x;
            this.v = this.motor.inputMoveDirection.y;
            if (this.h > 0 && this.v > 0) {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                      Quaternion.LookRotation(this.unit.transform.TransformDirection(rf)),
                                                      0.1f);
            } else if (this.h > 0 && this.v == 0) {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                      Quaternion.LookRotation(this.unit.transform.right),
                                                      0.1f);
            } else if ((this.h == 0 && this.v > 0) || this.v < 0) {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                      Quaternion.LookRotation(this.unit.transform.forward),
                                                      0.1f);
            } else if (this.h < 0 && this.v > 0) {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                      Quaternion.LookRotation(this.unit.transform.TransformDirection(lf)),
                                                      0.1f);
            } else if (this.h < 0 && this.v == 0) {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                      Quaternion.LookRotation(-this.unit.transform.right),
                                                      0.1f);
            }
        }
        if (this.motor.characterState == UnitMotor.CharacterState.Boosting) {
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                  Quaternion.LookRotation(this.unit.transform.forward),
                                                  0.1f);
        }
    }
}

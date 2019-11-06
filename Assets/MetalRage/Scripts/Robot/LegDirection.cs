using UnityEngine;

public class LegDirection : MonoBehaviour {
    [SerializeField]
    private GameObject unit = default;

    private UnitMotor motor;

    private void Start() {
        this.motor = this.unit.GetComponent<UnitMotor>();
    }

    private void Update() {
        var currentAngle = this.transform.localRotation.eulerAngles.y;
        currentAngle = currentAngle > 180f ? currentAngle - 360f : currentAngle;
        float destinationAngle = currentAngle;
        if (this.motor.characterState == RobotState.Walking) {
            var h = this.motor.inputMoveDirection.x;
            var v = this.motor.inputMoveDirection.y;
            destinationAngle = h > 0 && v > 0 ? 45f :
                              h > 0 && v == 0 ? 90f :
                   (h == 0 && v > 0) || v < 0 ? 0f :
                               h < 0 && v > 0 ? -45f :
                              h < 0 && v == 0 ? -90f : currentAngle;
        }
        if (this.motor.characterState == RobotState.Boosting) {
            destinationAngle = 0f;
        }
        var newAngle = Mathf.Lerp(currentAngle, destinationAngle, 5f * Time.deltaTime);
        this.transform.localRotation = Quaternion.AngleAxis(newAngle, Vector3.up);
    }
}

using UnityEngine;

[System.Serializable]
public class BulletSpread {
    [SerializeField]
    private float minAngle = 0f;
    [SerializeField]
    private float maxAngle = 10f;
    [Range(0f, 1f)]
    [SerializeField]
    private float growRate = 0.3f;

    public float Angle { get; set; }

    public float RadiusInScreen {
        get {
            var screenDistance = Screen.height / 2f / Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad);
            return screenDistance * Mathf.Tan(this.Angle * Mathf.Deg2Rad);
        }
    }

    public float MinAngle { get => this.minAngle; set => this.minAngle = value; }
    public float MaxAngle { get => this.maxAngle; set => this.maxAngle = value; }
    public float GrowRate { get => this.growRate; set => this.growRate = value; }

    public static float CorrectionFactor(MechMovementState state) {
        switch (state) {
            case MechMovementState.Stand:
                return 1f;
            case MechMovementState.Walking:
                return 1f;
            case MechMovementState.BoostAcceling:
                return 1.3f;
            case MechMovementState.BoostBraking:
                return 1.2f;
            case MechMovementState.Crouching:
                return 0.5f;
            case MechMovementState.Airborne:
                return 1.5f;
            default:
                return 1f;
        }
    }

    public void Spread() {
        this.Angle = Mathf.Min(this.MaxAngle, this.Angle + this.GrowRate * this.MaxAngle);
    }

    public void Suppress() {
        this.Angle = Mathf.Max(this.MinAngle, this.Angle - 3f * Time.deltaTime);
    }

    public void SuppressImmediate() {
        this.Angle = this.MinAngle;
    }

    public Vector2 GetSample(MechMovementState state) {
        var radius = Random.Range(0f, this.Angle * CorrectionFactor(state));
        var theta = Random.Range(0f, 2f * Mathf.PI);
        return new Vector2(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));
    }

    public Vector2 GetSampleInScreen(MechMovementState state) {
        var radius = Random.Range(0f, this.RadiusInScreen * CorrectionFactor(state));
        var theta = Random.Range(0f, 2f * Mathf.PI);
        return new Vector2(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));
    }
}

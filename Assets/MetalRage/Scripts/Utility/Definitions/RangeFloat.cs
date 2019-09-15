using UnityEngine;

[System.Serializable]
public struct RangeFloat {
    [SerializeField]
    private float min;
    [SerializeField]
    private float max;

    public float Min => this.min;
    public float Max => this.max;

    public RangeFloat(float min, float max) {
        this.min = min;
        this.max = max;
    }
}

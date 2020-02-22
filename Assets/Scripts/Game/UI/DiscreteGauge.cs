using UnityEngine;
using UnityEngine.UI;

public class DiscreteGauge : Image {
    public enum StepMode {
        Constant,
        Custom
    }

    //[SerializeField]
    //private Image image;
    [Range(1, 100)]
    [SerializeField]
    private int maxStepCount = 1;
    [SerializeField]
    private StepMode mode = StepMode.Constant;
    [SerializeField]
    private float[] fillRatios = default;
    [SerializeField]
    private int stepCount = 0;

    public int StepCount {
        get => this.stepCount;
        set {
            this.stepCount = Mathf.Clamp(value, 0, this.maxStepCount);
            switch (this.mode) {
                case StepMode.Constant:
                    this.fillAmount = (float)this.stepCount / this.MaxStepCount;
                    break;
                case StepMode.Custom:
                    this.fillAmount = this.FillRatios[this.stepCount];
                    break;
            }
        }
    }

    public StepMode Mode { get => this.mode; }
    public int MaxStepCount { get => this.maxStepCount; }
    public float[] FillRatios { get => this.fillRatios; }

}

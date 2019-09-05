using UnityEngine;
using UnityEngine.UI;

public class DiscreteGauge : MonoBehaviour {
    [SerializeField]
    private Image image;
    [SerializeField]
    private float[] fillAmounts;

    private int count = 0;

    private void Awake() {
        this.image = GetComponent<Image>();
    }

    public int MaxFillCount {
        get => this.fillAmounts.Length - 1;
    }

    public int FillCount {
        get => this.count;
        set {
            this.image.fillAmount = this.fillAmounts[value];
            this.count = value;
        }
    }
}

using UnityEngine;

public class RepairArmReticle : MonoBehaviour {
    [SerializeField]
    private DiscreteGauge fillGauge;
    [SerializeField]
    private GameObject hpIcon;
    [SerializeField]
    private GameObject highLight;

    public DiscreteGauge FillGauge { get => this.fillGauge; }

    public void ShowHPIcon() {
        this.hpIcon.SetActive(true);
    }

    public void HideHPIcon() {
        this.hpIcon.SetActive(false);
    }

    public void ShowHighLight() {
        this.highLight.SetActive(true);
    }

    public void HideHighLight() {
        this.highLight.SetActive(false);
    }
}

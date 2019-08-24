using UnityEngine;
using UnityEngine.UI;

public class StatusUI : CanvasUI {
    [SerializeField]
    private Text HPValue;
    [SerializeField]
    private Slider HPBar;
    [SerializeField]
    private Text magazineValue;
    [SerializeField]
    private Text ammoValue;
    [SerializeField]
    private Slider boostGauge;
    [SerializeField]
    private Text targetingEnemyNameText;
    [SerializeField]
    private Text targetingFriendHPText;

    public string TargetingEnemyName {
        get => this.targetingEnemyNameText.text;
        set => this.targetingEnemyNameText.text = value; }

    public Status TargetingFriend {
        set => this.targetingFriendHPText.text =
            value == null ? "" : value.HP.ToString();
    }

    public void SetBoostGauge(int boost) {
        this.boostGauge.value = boost / 100f;
    }

    public void SetHP(int hp, int maxHP) {
        this.HPValue.text = hp.ToString("D3");
        this.HPBar.value = hp / (float)maxHP;
    }

    public void SetMagazine(int magazine) {
        this.magazineValue.text = magazine.ToString("D3");
    }

    public void SetAmmo(int ammo) {
        this.ammoValue.text = ammo.ToString("D4");
    }
}
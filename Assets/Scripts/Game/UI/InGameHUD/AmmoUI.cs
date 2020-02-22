#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : CanvasUI {
    [SerializeField]
    private Text loadedBulletCountText;
    [SerializeField]
    private Text reserveBulletCountText;

    public void UpdateUI(Ammo ammo) {
        this.loadedBulletCountText.text = $"{ammo.LoadedBulletCount:000}";
        this.reserveBulletCountText.text = $"{ammo.ReserveBulletCount:000}";
    }
}
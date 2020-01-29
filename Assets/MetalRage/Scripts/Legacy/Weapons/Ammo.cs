using System;
using UnityEngine;

[Serializable]
public class Ammo {
    [SerializeField]
    private int magazineSize;
    [SerializeField]
    private int reserveBulletCount;

    private int loadedBulletCount;

    public int MagazineSize { get => this.magazineSize; set => this.magazineSize = value; }
    public bool CanReload => !this.IsMagazineFull && this.ReserveBulletCount != 0;
    public bool IsMagazineEmpty => this.LoadedBulletCount == 0;
    public bool IsMagazineFull => this.LoadedBulletCount == this.MagazineSize;

    public int LoadedBulletCount {
        get => this.loadedBulletCount;
        private set => this.loadedBulletCount = Mathf.Clamp(0, value, this.MagazineSize);
    }

    public int ReserveBulletCount {
        get => this.reserveBulletCount;
        set => this.reserveBulletCount = Mathf.Max(0, value);
    }

    public Ammo(int magazineSize, int reserveBulletCount) {
        this.MagazineSize = magazineSize;
        this.LoadedBulletCount = this.MagazineSize;
        this.ReserveBulletCount = reserveBulletCount;
    }

    public int Consume(int bulletCount) {
        var consumedBulletCount = Mathf.Min(this.LoadedBulletCount, bulletCount);
        this.LoadedBulletCount -= consumedBulletCount;
        return consumedBulletCount;
    }

    public int Supply(int bulletCount) {
        var suppliedBulletCount = Mathf.Min(this.MagazineSize - this.LoadedBulletCount, bulletCount);
        this.LoadedBulletCount += suppliedBulletCount;
        return suppliedBulletCount;
    }

    public void Reload() {
        int suppliedBulletCount = Mathf.Min(this.MagazineSize - this.LoadedBulletCount, this.reserveBulletCount);
        this.LoadedBulletCount += suppliedBulletCount;
        this.ReserveBulletCount -= suppliedBulletCount;
    }
}

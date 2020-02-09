using System.Collections;
using Unity.Entities;
using UnityEngine;

public struct WeaponConfigData : IComponentData {
    public int damage;
    public Vector2 MaxRecoil;// = new Vector2(3f, 14f);
    public float RecoilDuration;
    public float ReloadDuration;
    public float CoolDownDuration;
    public float SetupDuration;
    public float fireRate;
}

public struct WeaponStatus : IComponentData {
    public Bool IsFiring;
    public uint FireSeed;
    public double SetupStartTime;
    public double ReloadStartTime;
    public double FireTime;
    public Vector2 Recoil;
    public float Dispersion;
}

public class WeaponSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechCommand command, ref WeaponConfigData config, ref WeaponStatus status) => {
            status.FireTime += this.Time.DeltaTime;
            var isCoolDown = (this.Time.ElapsedTime - status.FireTime) < (1f / config.fireRate);
            if (command.Fire && !isCoolDown) {
                status.FireTime = this.Time.ElapsedTime;
                status.IsFiring = true;
                status.FireSeed = (uint)(this.Time.ElapsedTime % 10);
            } else {
                status.IsFiring = false;
            }
            if (command.Reload) {
                status.ReloadStartTime = this.Time.ElapsedTime;
            }
        });
    }
}

public class WeaponRecoilSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref WeaponConfigData config, ref WeaponStatus status) => {
            Vector2 nextRecoil;
            Vector2 deltaRecoil = new Vector2 {
                x = status.Recoil.x * (1f + status.Dispersion),
                y = status.Recoil.y * (1f + status.Dispersion)
            };
            if (status.Recoil.y <= config.MaxRecoil.y - 1f) {
                nextRecoil.y = status.Recoil.y + deltaRecoil.y;
            } else {
                nextRecoil.y = NextRecoilInRange(
                    status.FireSeed,
                    config.MaxRecoil.y - 1f,
                    config.MaxRecoil.y,
                    status.Recoil.y,
                    0.5f
                );
            }
            nextRecoil.x = NextRecoilInRange(
                status.FireSeed,
                -config.MaxRecoil.x,
                config.MaxRecoil.x,
                status.Recoil.x,
                deltaRecoil.x
            );
            var recoilTime = this.Time.ElapsedTime - status.FireTime;
            if (recoilTime < config.RecoilDuration) {
                var t = (float)recoilTime / config.RecoilDuration;
                status.Recoil = Vector2.Lerp(status.Recoil, nextRecoil, t);
            }
        });
    }

    private static float NextRecoilInRange(uint seed, float min, float max, float origin, float range) {
        var random = new Unity.Mathematics.Random(seed);
        var recoilMagnitude = random.NextFloat(0f, range);
        return (origin + recoilMagnitude < max)
            ? origin + recoilMagnitude : origin - recoilMagnitude;
    }
}

/// <summary>
/// Base class of weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour {
    public class Parameters {
        public int damage = 0;
        public Vector2 recoil;
        public Vector2 maxRecoil = new Vector2(3f, 14f);
        public float reloadTime = 0f;
        public float coolDownTime = 0f;
        public float setupTime = 1f;
        public bool isReloading = false;
    }

    [SerializeField]
    protected GameObject crosshairPrefab = default;
    [SerializeField]
    private Ammo ammo = default;
    [SerializeField]
    private BulletSpread spread = default;
    [SerializeField]
    private AudioClip setupClip = default;
    [SerializeField]
    private AudioClip reloadClip = default;

    protected Parameters param = new Parameters();
    protected WeaponRay ray;
    protected Mech robot;
    protected new AudioSource audio;

    public Mech Robot { get; set; }
    public Crosshair Crosshair { get; protected set; }
    public Ammo Ammo => this.ammo;
    public BulletSpread Spread => this.spread;
    public float SensitivityScale { get; protected set; } = 1f;

    protected void Initialize() {
        var crosshairObj = Instantiate(this.crosshairPrefab, Vector3.zero, Quaternion.identity);
        this.Crosshair = crosshairObj.GetComponentInChildren<Crosshair>();
        this.Crosshair.Hide();
    }

    protected void RecoilAndSpread() {
        this.spread.Spread();
    }

    protected void ConsumeBullets(int count) {
        this.Ammo.Consume(count);
        if (this.Ammo.CanReload && this.Ammo.LoadedBulletCount == 0) {
            StartCoroutine(ReloadRoutine());
        }
    }

    protected virtual IEnumerator ReloadRoutine() {
        this.audio.PlayOneShot(this.reloadClip);
        this.param.isReloading = true;
        yield return new WaitForSeconds(this.param.reloadTime);
        this.Ammo.Reload();
        this.param.isReloading = false;
    }

    public virtual void Select() {
        this.audio.PlayOneShot(this.setupClip);
        this.Crosshair?.Show();
        StopCoroutine(SelectRoutine());
        StartCoroutine(SelectRoutine());
    }

    public virtual void Unselect() {
        //this.robot.IsRecoiling = false;
        //this.param.isCoolDown = false;
        StopAllCoroutines();
        this.audio.Stop();
        InterruptReloading();
        this.enabled = false;
        if (this.Crosshair != null) {
            this.Crosshair.Hide();
        }
    }

    protected IEnumerator SelectRoutine() {
        yield return new WaitForSeconds(this.param.setupTime);
        this.enabled = true;
    }

    private void InterruptReloading() {
        this.param.isReloading = false;
    }
}

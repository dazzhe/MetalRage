using System;
using Unity.Entities;
using UnityEngine;

[Flags]
public enum MechActionMask {
    None = 0,
    All = -1,
    Crouch = 1,
    Jump = 2,
    Boost = 4
}

public struct Mech : IComponentData {
    public int MaxHP;
    public int HP;
    public int Armor;
    public Vector3 CameraTargetTranslation;
    public MechActionMask ActivatedActionMask;

    public void Initialize() {
        this.HP = this.MaxHP;
    }
}

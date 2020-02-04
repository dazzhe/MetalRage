using Unity.Entities;
using UnityEngine;

public struct Mech : IComponentData {
    public int MaxHP;
    public int HP;
    public int Armor;
    public Vector3 CameraTargetTranslation;

    public void Initialize() {
        this.HP = this.MaxHP;
    }
}

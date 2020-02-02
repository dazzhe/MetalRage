using Unity.Entities;
using UnityEngine;

public struct Mech {
    public Transform transform;
    public int MaxHP;
    public int HP;
    public int Armor;

    public void Initialize() {
        this.HP = this.MaxHP;
    }
}

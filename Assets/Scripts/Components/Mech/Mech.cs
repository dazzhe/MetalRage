using Unity.Entities;
using UnityEngine;

public struct Mech {
    public Transform
    public int MaxHP;
    public int HP;
    public int Armor;

    public void Initialize() {
        this.HP = this.MaxHP;
    }
}

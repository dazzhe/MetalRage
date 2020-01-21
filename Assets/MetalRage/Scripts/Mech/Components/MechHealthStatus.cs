using Unity.Entities;
using UnityEngine;

public class MechHealthStatus : IComponentData {
    [SerializeField]
    private int maxHP;
    [SerializeField]
    private int hp;
    [SerializeField]
    private int armor;

    public int MaxHP { get => this.maxHP; set => this.maxHP = value; }
    public int HP { get => this.hp; set => this.hp = value; }
    public int Armor { get => this.armor; set => this.armor = value; }

    public void Initialize() {
        this.HP = this.MaxHP;
    }
 }

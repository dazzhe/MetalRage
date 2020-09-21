using Unity.Entities;
using Unity.Mathematics;

public struct Mech : IComponentData {
    public int MaxHP;
    public int HP;
    public int Armor;
    public float3 CameraOffset;

    public void Initialize() {
        this.HP = this.MaxHP;
    }
}
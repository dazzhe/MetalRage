using Unity.Entities;

public class MechHealthStatus : IComponentData {
    public int MaxHP;
    public int HP;
    public int Armor;

    public void Initialize() {
        this.HP = this.MaxHP;
    }
}

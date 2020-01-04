using UnityEngine;

public class MechStatus : MonoBehaviour {
    [SerializeField]
    private int maxHP;

    public int HP { get; private set; }
    public int MaxHP { get => this.maxHP; set => this.maxHP = value; }
 
    private void Awake() {
        this.HP = this.MaxHP;
    }
}

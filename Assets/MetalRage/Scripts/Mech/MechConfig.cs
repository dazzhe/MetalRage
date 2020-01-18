using UnityEngine;

[CreateAssetMenu(fileName = "MechConfig", menuName = "MetalRage/Mech/Config")]
public class MechConfig : ScriptableObject {
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private MechActionConfigBase[] actions;

    public GameObject Prefab { get => this.prefab; set => this.prefab = value; }
    public MechActionConfigBase[] Actions { get => this.actions; set => this.actions = value; }
}

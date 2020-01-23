using UnityEngine;

[CreateAssetMenu(fileName = "MechConfig", menuName = "MetalRage/Mech/Config")]
public class MechConfig : ScriptableObject {
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private ActionConfigBase[] actions;

    public GameObject Prefab { get => this.prefab; set => this.prefab = value; }
    public ActionConfigBase[] Actions { get => this.actions; set => this.actions = value; }
}

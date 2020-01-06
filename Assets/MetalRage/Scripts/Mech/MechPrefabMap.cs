using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MechPrefabMap", menuName = "MetalRage/Mech/MechPrefabMap")]
public class MechPrefabMap : ScriptableObject {
    [System.Serializable]
    private struct MechPrefabEntry {
        public MechType type;
        public GameObject prefab;
    }

    [SerializeField]
    private MechPrefabEntry[] entries;

    public GameObject this[MechType type] =>
        this.entries.Where(e => e.type == type).FirstOrDefault().prefab;
}

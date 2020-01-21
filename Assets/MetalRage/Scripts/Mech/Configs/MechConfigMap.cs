using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MechConfigMap", menuName = "MetalRage/Mech/MechConfigMap")]
public class MechConfigMap : ScriptableObject {
    [System.Serializable]
    private struct MechConfigEntry {
        public MechType type;
        public MechConfig config;
    }

    [SerializeField]
    private MechConfigEntry[] entries;

    public MechConfig this[MechType type] =>
        this.entries.Where(e => e.type == type).FirstOrDefault().config;
}

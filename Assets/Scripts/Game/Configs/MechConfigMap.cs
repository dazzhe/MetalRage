using System.Collections.Generic;
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
    private MechConfigEntry[] entries = new MechConfigEntry[] {
        new MechConfigEntry{type = MechType.Vanguard, config = default },
        new MechConfigEntry{type = MechType.Dual, config = default },
        new MechConfigEntry{type = MechType.Blitz, config = default },
        new MechConfigEntry{type = MechType.Velox, config = default },
    };

    public MechConfig this[MechType type] =>
        this.entries.Where(e => e.type == type).FirstOrDefault().config;

    public List<MechConfig> GetAllConfigs() {
        return this.entries.Select(entry => entry.config).ToList();
    }
}

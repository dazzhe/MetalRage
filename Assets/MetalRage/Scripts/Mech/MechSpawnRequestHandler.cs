using UnityEngine;
using UnityEditor;
using Unity.Entities;

public class MechSpawnRequestHandler : ComponentSystem {
    private ComponentGroup group;

    protected override void OnCreateManager() {
        this.group = GetComponentGroup(typeof(MechSpawnRequest));
    }
    protected override void OnUpdate() {

    }
}
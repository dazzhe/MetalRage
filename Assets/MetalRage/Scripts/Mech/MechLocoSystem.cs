using Unity.Entities;
using UnityEngine;

public class MechLocoSystem : ComponentSystem {
    private ComponentGroup group;

    protected override void OnCreateManager() {
        base.OnCreateManager();
    }

    protected override void OnUpdate() {
        var locoParams = Game.Config.GetConfig<MechLocoParameters>();

        var characterControllers = this.group.GetComponentArray<CharacterController>();
        var mechs = this.group.GetComponentArray<Mech>();
        for (int i = 0; i < mechs.Length; ++i) {
            var characterController = characterControllers[i];
            var mech = mechs[i];

        }
    }
}

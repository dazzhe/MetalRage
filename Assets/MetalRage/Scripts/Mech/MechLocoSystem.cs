using Unity.Entities;
using UnityEngine;

public class MechLocoSystem : ComponentSystem {
    private ComponentGroup group;

    protected override void OnCreateManager() {
        base.OnCreateManager();
        this.group = GetComponentGroup(typeof(CharacterController), ComponentType.ReadOnly<MechLocoCommand>(), typeof(MechLocoStatus));
    }

    protected override void OnUpdate() {
        var characterControllers = this.group.GetComponentArray<CharacterController>();
        var commands = this.group.GetComponentDataArray<MechLocoCommand>();
        var locoStatuses = this.group.GetComponentDataArray<MechLocoStatus>();
        var entities = this.group.GetEntityArray();
        for (int i = 0; i < characterControllers.Length; ++i) {
            var locoStatus = locoStatuses[i];
            var prevPosition = characterControllers[i].transform.position;
            characterControllers[i].Move(commands[i].Motion);
            locoStatus.Velocity = (characterControllers[i].transform.position - prevPosition) / Time.deltaTime;
            locoStatus.State = commands[i].NextState;
            this.EntityManager.SetComponentData(entities[i], locoStatus);
        }
    }
}

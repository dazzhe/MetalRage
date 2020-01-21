using Unity.Entities;
using UnityEngine;

public class MechLocoSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((Entity entity, CharacterController characterController, ref MechLocoCommand command, ref MechLocoStatus status) => {
            var prevPosition = characterController.transform.position;
            characterController.Move(command.Motion);
            status.Velocity = (characterController.transform.position - prevPosition) / Time.deltaTime;
            status.State = command.NextState;
            this.EntityManager.SetComponentData(entity, status);
        });
    }
}

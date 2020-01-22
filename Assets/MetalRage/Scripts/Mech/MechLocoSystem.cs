using Unity.Entities;
using UnityEngine;

public class MechLocoSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((CharacterController characterController, ref MechLocoCommand command, ref MechLocoStatus status) => {
            var prevPosition = characterController.transform.position;
            characterController.Move(command.Motion - 9.8f * Vector3.up * Time.deltaTime);
            status.Velocity = (characterController.transform.position - prevPosition) / Time.deltaTime;
            status.State = command.NextState;
            status.LegYaw = Mathf.Lerp(status.LegYaw, command.LegYaw, 10f * Time.deltaTime);
        });
    }
}

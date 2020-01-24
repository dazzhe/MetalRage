using UnityEngine;

public class CrouchActionControlSystem : ActionControlSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechAction mechAction, ref CrouchActionConfigData config) => {
            if (!InputSystem.GetButton(MechCommandButton.Crouch)) {
                mechAction.IsReadyToExecute = false;
                return;
            }
            var locoState = this.EntityManager.GetComponentData<MechLocoStatus>(mechAction.Owner).State;
            var isValidState =
                locoState == MechLocoState.Acceling || locoState == MechLocoState.Braking ||
                locoState == MechLocoState.Crouching || locoState == MechLocoState.Idle ||
                locoState == MechLocoState.Walking;
            mechAction.IsReadyToExecute = isValidState;
        });
    }
}

public class CrouchActionSystem : ActionSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechAction mechAction, ref CrouchActionConfigData config) => {
            var status = this.EntityManager.GetComponentData<MechLocoStatus>(mechAction.Owner);
            var command = new MechLocoCommand {
                NextState = MechLocoState.Crouching,
                Motion = Vector3.zero,
                LegYaw = status.LegYaw
            };
            this.EntityManager.SetComponentData(mechAction.Owner, command);
        });
    }
}

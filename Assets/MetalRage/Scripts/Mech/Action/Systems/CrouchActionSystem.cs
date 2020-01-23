using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(MechActionActivator))]
public class CrouchActionActivationRequestSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechAction mechAction, ref CrouchActionConfigData config) => {
            if (InputSystem.GetButton(MechCommandButton.Crouch) && !mechAction.IsActive) {
                mechAction.State = ActionState.WaitingActivation;
            }
        });
    }
}

[UpdateAfter(typeof(MechActionActivator))]
public class CrouchActionSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechAction mechAction, ref CrouchActionConfigData config) => {
            if (!mechAction.IsActive || !InputSystem.GetButton(MechCommandButton.Crouch)) {
                mechAction.State = ActionState.Dormant;
                return;
            }
            mechAction.State = ActionState.Running;
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

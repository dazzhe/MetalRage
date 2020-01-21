using UnityEngine;
using System.Collections;
using Unity.Entities;

public class CrouchActionSystem : ComponentSystem {
    private EntityQuery group;

    protected override void OnCreateManager() {
        base.OnCreateManager();
        this.group = GetEntityQuery(typeof(MechAction), typeof(CrouchActionConfigData));
    }

    protected override void OnUpdate() {
         this.Entities.ForEach<MechAction, CrouchActionConfigData>(UpdateEntity);
    }

    private void UpdateEntity(ref MechAction mechAction, ref CrouchActionConfigData config) {
        if (!mechAction.IsActive) {
            return;
        }
        var locoStatus = this.EntityManager.GetComponentData<MechLocoStatus>(mechAction.Owner);
        if (locoStatus.IsOnGround && InputSystem.GetButton(MechCommandButton.Crouch)) {
            locoStatus.State = MechLocoState.Crouching;
            locoStatus.Velocity *= 0;
        }
        //this.animator.SetBool("IsCrouching", this.locoState == MechLocoState.Crouching);
    }
}

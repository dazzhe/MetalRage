using Unity.Entities;
using UnityEngine;

class WalkActionSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechAction mechAction, ref WalkActionConfigData config) => {
            if (!mechAction.IsActive) {
                mechAction.State = ActionState.WaitingActivation;
                return;
            }
            var locoStatus = this.EntityManager.GetComponentData<MechLocoStatus>(mechAction.Owner);
            //if (!locoStatus.IsOnGround) {
            //    return;
            //}
            var motion = new Vector3(locoStatus.Velocity.x, 0f, locoStatus.Velocity.z);
            motion.x += InputSystem.GetMoveHorizontal() * config.Accel * Time.deltaTime;
            motion.z += InputSystem.GetMoveVertical() * config.Accel * Time.deltaTime;
            if (InputSystem.GetMoveHorizontal() == 0) {
                var xAbs = Mathf.Max(Mathf.Abs(motion.x) - config.Decel * Time.deltaTime, 0f);
                motion.x = motion.x > 0 ? xAbs : -xAbs;
            }
            if (InputSystem.GetMoveVertical() == 0) {
                var zAbs = Mathf.Max(Mathf.Abs(motion.z) - config.Decel * Time.deltaTime, 0f);
                motion.z = motion.z > 0 ? zAbs : -zAbs;
            }
            motion = motion.magnitude > config.MaxSpeed ? motion.normalized * config.MaxSpeed : motion;
            var command = new MechLocoCommand {
                Motion = motion * Time.deltaTime,
                NextState = motion.magnitude == 0 ? MechLocoState.Idle : MechLocoState.Walking
            };
            var x = InputSystem.GetMoveHorizontal();
            var z = InputSystem.GetMoveVertical();
            command.LegYaw = x > 0 && z > 0 ? 45f :
                          x > 0 && z == 0 ? 90f :
               (x == 0 && z > 0) || z < 0 ? 0f :
                           x < 0 && z > 0 ? -45f :
                          x < 0 && z == 0 ? -90f : locoStatus.LegYaw;
            this.EntityManager.SetComponentData(mechAction.Owner, command);
        });
    }
}

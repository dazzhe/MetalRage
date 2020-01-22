using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(MechLocoSystem))]
public class MechAnimationSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechLocoStatus status, Animator animator) => {
            animator.SetFloat("WalkSpeed", status.Velocity.magnitude);
            animator.SetBool("IsCrouching", status.State == MechLocoState.Crouching);
            animator.SetBool("IsOnGround", true);
            animator.SetFloat("LegOffsetYaw", status.LegYaw);
        });
    }
}

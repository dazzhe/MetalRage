using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(MechMovementSystem))]
public class MechAnimationSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechMovementStatus status, Animator animator) => {
            animator.SetFloat("WalkSpeed", status.Velocity.magnitude);
            animator.SetBool("IsCrouching", status.State == MechMovementState.Crouching);
            animator.SetBool("IsOnGround", true);
            animator.SetFloat("LegOffsetYaw", status.LegYaw);
            animator.SetFloat("AimOffsetPitch", status.Pitch);
        });
    }
}

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(MechMovementUpdateSystem))]
public class MechAnimationSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechMovementStatus status, Animator animator) => {
            animator.SetFloat("WalkSpeed", math.length(status.Velocity));
            animator.SetBool("IsCrouching", status.State == MechMovementState.Crouching);
            animator.SetBool("IsOnGround", status.State != MechMovementState.Airborne);
            animator.SetFloat("LegOffsetYaw", status.LegYaw);
            animator.SetFloat("AimOffsetPitch", status.Pitch);
        });
    }
}

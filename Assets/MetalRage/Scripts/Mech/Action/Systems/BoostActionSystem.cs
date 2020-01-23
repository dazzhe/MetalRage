using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(MechActionActivator))]
class BoostActionActivationRequestSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechAction mechAction, ref BoostActionConfigData config) => {
            if (InputSystem.GetButtonDown(MechCommandButton.Boost)) {
                mechAction.State = ActionState.Dormant;
                return;
            }
            var engine = this.EntityManager.GetComponentData<BoostEngineStatus>(mechAction.Owner);
            if (engine.Gauge >= config.Consumption) {
                mechAction.State = ActionState.WaitingActivation;
            } else {
                mechAction.State = ActionState.Dormant;
            }
        });
    }
}

[UpdateAfter(typeof(MechActionActivator))]
class BoostActionSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((ref MechAction mechAction, ref WalkActionConfigData config) => {
            if (!mechAction.IsActive) {
                return;
            }
            var status = this.EntityManager.GetComponentData<MechLocoStatus>(mechAction.Owner);
            var engine = this.EntityManager.GetComponentData<BoostEngineStatus>(mechAction.Owner);
            var canBoost = InputSystem.GetButtonDown(MechCommandButton.Boost) && engine.Gauge >= 28;
            if (canBoost) {
                engine.Gauge -= 28;
                float h = InputSystem.GetMoveHorizontal();
                float v = InputSystem.GetMoveVertical();
                var boostDirection = Vector3.forward;
                if (v >= 0 && h != 0) {
                    boostDirection = status.Velocity.normalized;
                }
                //this.engine.ShowJetFlame(0.4f, Vector3.forward);
                //StartCoroutine(BoostStart());
            }
            if (status.State == MechLocoState.Boosting) {
                Boost();
            }
            if (status.State == MechLocoState.Braking) {
                BoostBrake();
            }
        });

        private IEnumerator BoostStart() {
            // Jump and squat inputs are accepted only if MechLocoState is "Acceling".
            this.MoveDirection = this.boostDirection * this.boostSpeed;
            this.locoState = MechLocoState.Acceling;
            yield return new WaitForSeconds(0.05f);
            if (this.locoState == MechLocoState.Acceling) {
                this.locoState = MechLocoState.Boosting;
                yield return new WaitForSeconds(this.boostTime);
                this.locoState = MechLocoState.Braking;
            }
        }

        private void Boost() {
            // Stop movement if the unit collides to a wall.
            if ((this.controller.collisionFlags & CollisionFlags.Sides) != 0) {
                this.MoveDirection = Vector3.zero;
            }
        }

        private void BoostBrake() {
            this.MoveDirection = Vector3.Lerp(this.MoveDirection, Vector3.zero, 6f * Time.deltaTime);
            if (this.MoveDirection.magnitude <= this.walkSpeed) {
                this.locoState = MechLocoState.Walking;
            }
        }
    }

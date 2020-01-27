using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public struct MechPlayerInput : IComponentData {
    public bool Fire;
    public bool Crouch;
    public bool Boost;
    public bool TriggerBoost;
    public bool Jump;
    public Vector2 Move;
}

[AlwaysUpdateSystem]
public class MechInputSystem : ComponentSystem, Controls.IMechActions {
    private MechPlayerInput input = new MechPlayerInput();
    private Controls controls;
    private EntityQuery query;

    protected override void OnCreate() {
        this.controls = new Controls();
        this.controls.Mech.SetCallbacks(this);
        this.query = GetEntityQuery(typeof(MechPlayerInput));
    }

    protected override void OnUpdate() {
        if (this.query.CalculateEntityCount() == 0) {
            this.EntityManager.CreateEntity(typeof(MechPlayerInput));
        }
        this.query.SetSingleton(this.input);
    }

    protected override void OnStartRunning() => this.controls.Enable();
    protected override void OnStopRunning() => this.controls.Disable();

    public void OnBoost(InputAction.CallbackContext context) => this.input.Boost = context.ReadValue<bool>();
    public void OnCrouch(InputAction.CallbackContext context) => context.ReadValue<bool>();
    public void OnFire(InputAction.CallbackContext context) => context.ReadValue<bool>();
    public void OnJump(InputAction.CallbackContext context) => this.input.Jump = context.ReadValue<bool>();
    public void OnLook(InputAction.CallbackContext context) => context.ReadValue<Vector2>();
    public void OnMove(InputAction.CallbackContext context) => context.ReadValue<Vector2>();
    public void OnTriggerBoost(InputAction.CallbackContext context) => this.input.TriggerBoost = context.ReadValue<bool>();
}

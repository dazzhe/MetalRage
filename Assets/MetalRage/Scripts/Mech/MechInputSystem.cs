using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public struct PlayerInputData : IComponentData {
    public bool Fire;
    public bool Crouch;
    public bool Boost;
    public bool BoostOneShot;
    public bool Jump;
    public Vector2 Move;
    public Vector2 DeltaLook;
}

[AlwaysUpdateSystem]
public class MechInputSystem : ComponentSystem, Controls.IMechActions {
    private PlayerInputData prevInput = new PlayerInputData();
    private PlayerInputData input = new PlayerInputData();
    private Controls controls;
    private EntityQuery query;

    protected override void OnCreate() {
        this.controls = new Controls();
        this.controls.Mech.SetCallbacks(this);
        this.query = GetEntityQuery(typeof(PlayerInputData));
    }

    protected override void OnUpdate() {
        if (this.query.CalculateEntityCount() == 0) {
            this.EntityManager.CreateEntity(typeof(PlayerInputData));
        }
        var newInput = this.input;
        newInput.BoostOneShot = this.input.Boost && !this.prevInput.Boost;
        newInput.Jump = this.input.Jump && !this.prevInput.Jump;
        this.query.SetSingleton(newInput);
        this.prevInput = this.input;
    }

    protected override void OnStartRunning() => this.controls.Enable();
    protected override void OnStopRunning() => this.controls.Disable();

    public void OnBoost(InputAction.CallbackContext context) =>
        this.input.Boost = context.ReadValue<float>() > 0f;
    public void OnCrouch(InputAction.CallbackContext context) =>
        this.input.Crouch = context.ReadValue<float>() > 0f;
    public void OnFire(InputAction.CallbackContext context) =>
        this.input.Fire = context.ReadValue<float>() > 0f;
    public void OnJump(InputAction.CallbackContext context) =>
        this.input.Jump = context.ReadValue<float>() > 0f;
    public void OnLook(InputAction.CallbackContext context) =>
        this.input.DeltaLook = context.ReadValue<Vector2>();
    public void OnMove(InputAction.CallbackContext context) =>
        this.input.Move = context.ReadValue<Vector2>();
}

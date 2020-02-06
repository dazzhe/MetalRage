using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateAfter(typeof(PlayerInputSystem))]
public class MechCommandSystem : ComponentSystem {
    private EntityQuery playerInputQuery;

    protected override void OnCreate() {
        this.playerInputQuery = GetEntityQuery(typeof(PlayerInputData));
    }

    protected override void OnUpdate() {
        var input = this.playerInputQuery.GetSingleton<PlayerInputData>();
        this.Entities.ForEach((ref MechCommand command) => {
            command = new MechCommand {
                Fire = input.Fire,
                Crouch = input.Crouch,
                Boost = input.Boost,
                BoostOneShot = input.BoostOneShot,
                Jump = input.Jump,
                Move = input.Move,
                DeltaLook = input.DeltaLook,
                LeanLeft = input.LeanLeft,
                LeanRight = input.LeanRight
            };
        });
    }
}

[AlwaysUpdateSystem]
public class PlayerInputSystem : ComponentSystem, Controls.IMechActions {
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
        newInput.LeanLeft = this.input.LeanLeft && !this.prevInput.LeanLeft;
        newInput.LeanRight = this.input.LeanRight && !this.prevInput.LeanRight;
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
    public void OnLeanLeft(InputAction.CallbackContext context) =>
        this.input.LeanLeft = context.ReadValue<float>() > 0f;
    public void OnLeanRight(InputAction.CallbackContext context) =>
        this.input.LeanRight = context.ReadValue<float>() > 0f;
}

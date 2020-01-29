// GENERATED AUTOMATICALLY FROM 'Assets/MetalRage/Configs/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Mech"",
            ""id"": ""265c38f5-dd18-4d34-b198-aec58e1627ff"",
            ""actions"": [
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""1077f913-a9f9-41b1-acb3-b9ee0adbc744"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap,SlowTap""
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""50fd2809-3aa3-4a90-988e-1facf6773553"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""c60e0974-d140-4597-a40e-9862193067e9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""8fd8feff-68dc-4306-9e74-64c045b938bf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""799bad5a-41f7-4bf5-b3f4-5a24d5ee6259"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Boost"",
                    ""type"": ""Button"",
                    ""id"": ""1216e0b5-b6ec-4814-8e0d-7beb9096f57a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""abb776f3-f329-4f7b-bbf8-b577d13be018"",
                    ""path"": ""*/{PrimaryAction}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e1b8c4dd-7b3a-4db6-a93a-0889b59b1afc"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Dpad"",
                    ""id"": ""cefc16fc-557a-44b0-939f-2ad792876b07"",
                    ""path"": ""Dpad(normalize=false)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""07244659-79df-461d-b329-defbe2fbc5f6"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f0ec75cb-f02c-40d2-a33f-1fd6eab2ae0b"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""21fe6bfe-4721-4483-9f4a-a0031ade105c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2dd39746-c75c-4a11-838a-e59eacaf4e0b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c106d6e6-2780-47ff-b318-396171bd54cc"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""578caa03-6827-4797-adfc-a59770c437fe"",
                    ""path"": ""<Pointer>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e2ef2364-3b25-4791-b690-44afcf078499"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2d4bf7dd-915a-4b7e-ab3d-180296acbe81"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce93b4ec-41bd-4c4a-a9e0-5a1ae0a1698b"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Mech
        m_Mech = asset.FindActionMap("Mech", throwIfNotFound: true);
        m_Mech_Fire = m_Mech.FindAction("Fire", throwIfNotFound: true);
        m_Mech_Move = m_Mech.FindAction("Move", throwIfNotFound: true);
        m_Mech_Look = m_Mech.FindAction("Look", throwIfNotFound: true);
        m_Mech_Crouch = m_Mech.FindAction("Crouch", throwIfNotFound: true);
        m_Mech_Jump = m_Mech.FindAction("Jump", throwIfNotFound: true);
        m_Mech_Boost = m_Mech.FindAction("Boost", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Mech
    private readonly InputActionMap m_Mech;
    private IMechActions m_MechActionsCallbackInterface;
    private readonly InputAction m_Mech_Fire;
    private readonly InputAction m_Mech_Move;
    private readonly InputAction m_Mech_Look;
    private readonly InputAction m_Mech_Crouch;
    private readonly InputAction m_Mech_Jump;
    private readonly InputAction m_Mech_Boost;
    public struct MechActions
    {
        private @Controls m_Wrapper;
        public MechActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Fire => m_Wrapper.m_Mech_Fire;
        public InputAction @Move => m_Wrapper.m_Mech_Move;
        public InputAction @Look => m_Wrapper.m_Mech_Look;
        public InputAction @Crouch => m_Wrapper.m_Mech_Crouch;
        public InputAction @Jump => m_Wrapper.m_Mech_Jump;
        public InputAction @Boost => m_Wrapper.m_Mech_Boost;
        public InputActionMap Get() { return m_Wrapper.m_Mech; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MechActions set) { return set.Get(); }
        public void SetCallbacks(IMechActions instance)
        {
            if (m_Wrapper.m_MechActionsCallbackInterface != null)
            {
                @Fire.started -= m_Wrapper.m_MechActionsCallbackInterface.OnFire;
                @Fire.performed -= m_Wrapper.m_MechActionsCallbackInterface.OnFire;
                @Fire.canceled -= m_Wrapper.m_MechActionsCallbackInterface.OnFire;
                @Move.started -= m_Wrapper.m_MechActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_MechActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_MechActionsCallbackInterface.OnMove;
                @Look.started -= m_Wrapper.m_MechActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_MechActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_MechActionsCallbackInterface.OnLook;
                @Crouch.started -= m_Wrapper.m_MechActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_MechActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_MechActionsCallbackInterface.OnCrouch;
                @Jump.started -= m_Wrapper.m_MechActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_MechActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_MechActionsCallbackInterface.OnJump;
                @Boost.started -= m_Wrapper.m_MechActionsCallbackInterface.OnBoost;
                @Boost.performed -= m_Wrapper.m_MechActionsCallbackInterface.OnBoost;
                @Boost.canceled -= m_Wrapper.m_MechActionsCallbackInterface.OnBoost;
            }
            m_Wrapper.m_MechActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Fire.started += instance.OnFire;
                @Fire.performed += instance.OnFire;
                @Fire.canceled += instance.OnFire;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Boost.started += instance.OnBoost;
                @Boost.performed += instance.OnBoost;
                @Boost.canceled += instance.OnBoost;
            }
        }
    }
    public MechActions @Mech => new MechActions(this);
    public interface IMechActions
    {
        void OnFire(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnBoost(InputAction.CallbackContext context);
    }
}

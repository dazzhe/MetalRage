using UnityEngine;

public enum MechCommandButton : ushort {
    Fire1 = 1,
    Fire2 = 1 << 1,
    Reload = 1 << 2,
    MainWeapon = 1 << 3,
    LeftShoulderWeapon = 1 << 4,
    RightShoulderWeapon = 1 << 5,
    Boost = 1 << 6,
    Jump = 1 << 7,
    Crouch = 1 << 8
}

public static class InputSystem {
    private static bool IsControllable => UIManager.Instance.MenuUI.ActiveWindowLevel == 0;

    public static string GetButtonName(MechCommandButton button) {
        switch (button) {
            case MechCommandButton.Boost: return "Boost";
            case MechCommandButton.Fire1: return "Fire1";
            case MechCommandButton.Fire2: return "Fire2";
            case MechCommandButton.Jump: return "Jump";
            case MechCommandButton.LeftShoulderWeapon: return "LeftShoulderWeapon";
            case MechCommandButton.RightShoulderWeapon: return "RightShoulderWeapon";
            case MechCommandButton.Reload: return "Reload";
            case MechCommandButton.Crouch: return "Crouch";
            default: return "";
        }
    }

    public static bool GetButton(MechCommandButton button) {
        var buttonName = GetButtonName(button);
        return IsControllable ? Input.GetButton(buttonName) : false;
    }

    public static bool GetButtonDown(MechCommandButton button) {
        var buttonName = GetButtonName(button);
        return IsControllable ? Input.GetButtonDown(buttonName) : false;
    }

    public static float GetHorizontalMotion() {
        return IsControllable ? Input.GetAxisRaw("Horizontal") : 0f;
    }

    public static float GetVerticalMotion() {
        return IsControllable ? Input.GetAxisRaw("Vertical") : 0f;
    }

    public static float GetMouseX() {
        return IsControllable ? Input.GetAxis("Mouse X") : 0f;
    }

    public static float GetMouseY() {
        return IsControllable ? Input.GetAxis("Mouse Y") : 0f;
    }
}

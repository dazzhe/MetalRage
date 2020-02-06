using Unity.Entities;
using UnityEngine;

public struct MechCommand : IComponentData {
    public bool Fire;
    public bool Crouch;
    public bool Boost;
    public bool BoostOneShot;
    public bool Jump;
    public Vector2 Move;
    public Vector2 DeltaLook;
    public bool LeanLeft;
    public bool LeanRight;
}

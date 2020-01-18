using Unity.Entities;
using UnityEngine;

public enum ActionState {
    Idle,
    RequestingActivation,
    Active,
    Cooldown,
}

public struct MechAction : IComponentData {
    [SerializeField]
    private Entity owner;
    [SerializeField]
    private ActionTag tag;
    [SerializeField]
    private ActionTag blockingTag;
    [SerializeField]
    private ActionTag interruptibleTag;
    [SerializeField]
    private ActionState state;
    [SerializeField]
    private byte isActive;
    [SerializeField]
    private byte isDeactivationRequested;

    public Entity Owner { get => this.owner; set => this.owner = value; }
    public ActionTag Tag { get => this.tag; set => this.tag = value; }
    public ActionTag BlockingTag { get => this.blockingTag; set => this.blockingTag = value; }
    public ActionTag InterruptibleTag { get => this.interruptibleTag; set => this.interruptibleTag = value; }
    public ActionState State { get => this.state; set => this.state = value; }
    public bool IsActive { get => this.isActive == 1; set => this.isActive = value ? (byte)1 : (byte)0; }
    public bool IsDeactivationRequested { get => this.isDeactivationRequested == 1; set => this.isDeactivationRequested = value ? (byte)1 : (byte)0; }
}

[InternalBufferCapacity(8)]
public struct MechActionEntity : IBufferElementData {
    [SerializeField]
    private Entity value;

    public Entity Value { get => this.value; set => this.value = value; }
}


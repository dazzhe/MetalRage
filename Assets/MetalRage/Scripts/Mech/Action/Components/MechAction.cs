using Unity.Entities;

public struct MechAction : IComponentData {
    public Entity Owner;
    /// <summary>
    /// Set by each <see cref="ActionControlSystem"/>(e.g. <see cref="WalkActionControlSystem"/>) before update of <see cref="ActionGroupControlSystem"/>.
    /// </summary>
    public BlittableBool IsReadyToExecute;
    /// <summary>
    /// Set by <see cref="ActionGroupControlSystem"/> before update of <see cref="ActionSystem"/>(e.g. <see cref="WalkActionSystem"/>).<br/>
    /// If <see cref="IsReadyToExecute"/> is true and the action is not restricted by <see cref="ActionExecutionConstraint"/>, <see cref="IsExecutionAllowed"/> is set to true.<br/>
    /// <see cref="ActionSystem"/> is not allowed to execute if <see cref="IsExecutionAllowed"/> is false.
    /// </summary>
    public BlittableBool IsExecutionAllowed;
    /// <summary>
    /// Set by <see cref="ActionGroupControlSystem"/> before update of <see cref="ActionSystem"/>(e.g. <see cref="WalkActionSystem"/>).<br/>
    /// If the action is blocking another action, <see cref="IsInterruptionRequested"/> is set to true.<br/>
    /// Use <see cref="IsInterruptionRequested"/> if you want to gracefully terminate the action.
    /// </summary>
    public BlittableBool IsInterruptionRequested;
}

[InternalBufferCapacity(8)]
public struct MechActionEntity : IBufferElementData {
    public Entity Value;
}

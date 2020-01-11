using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class MechActionConfiguration {
    [SerializeField]
    private MechActionFactory factory;
    [SerializeField]
    private MechActionFactory[] nonBlockingActions;
    [SerializeField]
    private bool canInterruptAll;
    [SerializeField]
    private MechActionFactory[] interruptibleActions;

    public MechActionFactory Factory { get => this.factory; set => this.factory = value; }
    public MechActionFactory[] InterruptibleActions { get => this.nonBlockingActions; set => this.nonBlockingActions = value; }
    public bool CanInterruptAll { get => this.canInterruptAll; set => this.canInterruptAll = value; }
    public MechActionFactory[] Interruptables { get => this.interruptibleActions; set => this.interruptibleActions = value; }
}

[InternalBufferCapacity(8)]
public struct MechActionBufferElement : IBufferElementData {
    public Entity entity;
    public uint nonBlockingActionMask;
    public uint interruptibleActionMask;
}

[CreateAssetMenu(fileName = "ActionCollection", menuName = "MetalRage/Mech/ActionCollection")]
public class MechActionCollection : ScriptableObject {
    public MechActionConfiguration[] actionConfigs;

    public Entity CreateEntity(EntityManager entityManager) {
        var entity = entityManager.CreateEntity();
        var abilityEntities = new List<Entity>(this.actionConfigs.Length);
        for (int i = 0; i < this.actionConfigs.Length; i++) {
            abilityEntities.Add(this.actionConfigs[i].Factory.Create(entityManager));
        }
        // Add abilities to ability buffer
        var actionBuffer = entityManager.AddBuffer<MechActionBufferElement>(entity);
        for (int i = 0; i < this.actionConfigs.Length; i++) {
            uint canRunWith = 0;
            foreach (var ability in this.actionConfigs[i].InterruptibleActions) {
                var abilityIndex = GetAbilityIndex(ability);
                canRunWith |= 1U << abilityIndex;
            }
            uint canInterrupt = 0;
            if (this.actionConfigs[i].CanInterruptAll) {
                canInterrupt = ~0U;
            } else {
                foreach (var ability in this.actionConfigs[i].InterruptibleActions) {
                    var abilityIndex = GetAbilityIndex(ability);
                    canInterrupt |= 1U << abilityIndex;
                }
            }
            actionBuffer.Add(new MechActionBufferElement {
                entity = abilityEntities[i],
                nonBlockingActionMask = canRunWith,
                interruptibleActionMask = canInterrupt,
            });
        }
        return entity;
    }

    int GetAbilityIndex(MechActionFactory factory) {
        for (int i = 0; i < this.actionConfigs.Length; i++) {
            if (this.actionConfigs[i].Factory == factory)
                return i;
        }
        return -1;
    }
}

[DisableAutoCreation]
class DefaultBehaviourController_Update : ComponentSystem {
    private ComponentGroup group;

    protected override void OnCreateManager() {
        base.OnCreateManager();
        this.group = GetComponentGroup(ComponentType.ReadOnly<MechActionBufferElement>());
    }

    protected override void OnUpdate() {
        var actionBuffers = this.group.GetBufferArray<MechActionBufferElement>();
        for (int i = 0; i < actionBuffers.Length; ++i) {
            UpdateMech(actionBuffers[i]);
        }
    }

    private void UpdateMech(DynamicBuffer<MechActionBufferElement> actionBuffer) {
        // Check for abilities done
        for (int i = 0; i < actionBuffer.Length; ++i) {
            var actionEntity = actionBuffer[i].entity;
            var abilityCtrl = EntityManager.GetComponentData<>(ability);
            if (abilityCtrl.active == 1 && abilityCtrl.behaviorState != AbilityControl.State.Active) {
                //                GameDebug.Log("Behavior done:" + ability);
                Deactivate(ability);
            }
        }

        // Get active abilties
        uint activeAbilityFlags = 0;
        for (int i = 0; i < abilityEntries.Length; i++) {
            var ability = abilityEntries[i].ability;
            var abilityCtrl = EntityManager.GetComponentData<AbilityControl>(ability);

            if (abilityCtrl.active == 1)
                activeAbilityFlags |= 1U << i;
        }

        // Check for ability activate
        for (int i = 0; i < abilityEntries.Length; i++) {
            var abilityEntry = abilityEntries[i];

            var abilityCtrl = EntityManager.GetComponentData<AbilityControl>(abilityEntry.ability);
            if (abilityCtrl.active == 1)
                continue;

            if (abilityCtrl.behaviorState != AbilityControl.State.RequestActive)
                continue;

            //            if(DefaultCharBehaviourController.ShowInfo.IntValue > 0)
            //                GameDebug.Log("AttempActivate:" + ability);


            var canNotRunWith = activeAbilityFlags & ~abilityEntries[i].canRunWith;
            if (activeAbilityFlags == 0 || canNotRunWith == 0) {
                Activate(abilityEntry.ability);
                continue;
            }

            var canActivate = true;
            for (int j = 0; j < abilityEntries.Length; j++) {
                var flag = 1U << j;
                var blocking = (canNotRunWith & flag) > 0;
                if (blocking) {
                    var canInterrupt = (abilityEntry.canInterrupt & flag) > 0;
                    if (canInterrupt) {
                        Deactivate(abilityEntries[j].ability);
                    } else {
                        RequestDeactivate(abilityEntries[j].ability);
                        canActivate = false;
                    }
                }
            }

            if (canActivate) {
                Activate(abilityEntry.ability);
            }
        }
    }

    void Activate(Entity behaviour) {
        var abilityCtrl = EntityManager.GetComponentData<BehaviourControl>(behaviour);
        abilityCtrl.active = 1;
        abilityCtrl.requestDeactivate = 0;
        EntityManager.SetComponentData(behaviour, abilityCtrl);
    }

    void Deactivate(Entity ability) {
        var abilityCtrl = EntityManager.GetComponentData<AbilityControl>(ability);
        abilityCtrl.active = 0;
        EntityManager.SetComponentData(ability, abilityCtrl);
    }

    void RequestDeactivate(Entity ability) {
        var abilityCtrl = EntityManager.GetComponentData<AbilityControl>(ability);
        abilityCtrl.requestDeactivate = 1;
        EntityManager.SetComponentData(ability, abilityCtrl);
    }
}

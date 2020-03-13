using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(MechMovementRequestSystem))]
public class MechRotationUpdateSystem : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((
            ref Rotation rotation,
            ref MechMovementStatus status,
            ref MechCommand command,
            ref MechMovementConfigData config
        ) => {
            status.Yaw += math.radians(command.DeltaLook.x * Preferences.Sensitivity.GetFloat());
            status.Pitch -= command.DeltaLook.y * Preferences.Sensitivity.GetFloat();
            status.Pitch = math.clamp(status.Pitch, config.MinPitch, config.MaxPitch);
            rotation.Value = quaternion.Euler(new float3(0f, status.Yaw, 0f));
        });
    }
}

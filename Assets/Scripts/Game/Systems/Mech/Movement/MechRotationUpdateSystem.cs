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
            var deltaLook = math.radians(command.DeltaLook);
            var maxPitch = math.radians(config.MaxPitch);
            status.Yaw += deltaLook.x * Preferences.Sensitivity.GetFloat();
            status.Pitch -= deltaLook.y * Preferences.Sensitivity.GetFloat();
            status.Pitch = math.clamp(status.Pitch, -maxPitch, maxPitch);
        });
    }
}

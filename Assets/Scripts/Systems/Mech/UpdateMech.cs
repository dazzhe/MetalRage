using Unity.Entities;
using UnityEngine;

public class UpdateMech : ComponentSystem {
    protected override void OnUpdate() {
        this.Entities.ForEach((MechComponent mech) => {
            UpdateElevation(mech);
            //UIManager.Instance.StatusUI.SetHP(mech.mechStatus.HP, mech.mechStatus.MaxHP);
            //UIManager.Instance.StatusUI.SetBoostGauge(mech.mechMotor.BoostGauge);
        });
    }

    private void UpdateElevation(MechComponent mech) {
        mech.BaseRotationY += Input.GetAxis("Mouse Y") * Preferences.Sensitivity.GetFloat() * mech.SensitivityScale;
        mech.BaseRotationY = Mathf.Clamp(mech.BaseRotationY, mech.ElevationRange.Min, mech.ElevationRange.Max);
        mech.RotationY = mech.BaseRotationY + mech.RecoilRotation.y;
        mech.CameraFollowTarget.localRotation = Quaternion.AngleAxis(-mech.RotationY, Vector3.right);
    }
}

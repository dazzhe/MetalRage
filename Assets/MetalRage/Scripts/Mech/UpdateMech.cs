using Unity.Entities;
using UnityEngine;

public class UpdateMech : ComponentSystem {
    private ComponentGroup group;

    protected override void OnCreateManager() {
        this.group = GetComponentGroup(typeof(Mech));
    }

    protected override void OnUpdate() {
        var mechs = this.group.GetComponentArray<Mech>();
        for (int i = 0; i < mechs.Length; ++i) {
            var mech = mechs[i];
            UpdateElevation(ref mech);
            //UIManager.Instance.StatusUI.SetHP(mech.mechStatus.HP, mech.mechStatus.MaxHP);
            //UIManager.Instance.StatusUI.SetBoostGauge(mech.mechMotor.BoostGauge);
        }
    }

    private void UpdateElevation(ref Mech mech) {
        mech.BaseRotationY += Input.GetAxis("Mouse Y") * Configuration.Sensitivity.GetFloat() * mech.SensitivityScale;
        mech.BaseRotationY = Mathf.Clamp(mech.BaseRotationY, mech.ElevationRange.Min, mech.ElevationRange.Max);
        mech.RotationY = mech.BaseRotationY + mech.RecoilRotation.y;
        mech.CameraFollowTarget.localRotation = Quaternion.AngleAxis(-mech.RotationY, Vector3.right);
    }
}

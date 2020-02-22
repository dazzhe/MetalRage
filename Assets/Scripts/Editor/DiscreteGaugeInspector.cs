using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DiscreteGauge))]
public class DiscreteGaugeInspector : Editor {
    private bool isFillRatiosFolded = false;
    private bool isFillParamsFolded = false;
    private SerializedProperty spriteProperty;
    private SerializedProperty stepCountProperty;
    private SerializedProperty maxStepCountProperty;
    private SerializedProperty modeProperty;
    private SerializedProperty fillRatiosProperty;
    private SerializedProperty fillMethodProperty;
    private SerializedProperty fillAmountProperty;
    private SerializedProperty fillOriginProperty;
    private SerializedProperty typeProperty;

    private void OnEnable() {
        this.spriteProperty = this.serializedObject.FindProperty("m_Sprite");
        this.stepCountProperty = this.serializedObject.FindProperty("stepCount");
        this.maxStepCountProperty = this.serializedObject.FindProperty("maxStepCount");
        this.modeProperty = this.serializedObject.FindProperty("mode");
        this.fillRatiosProperty = this.serializedObject.FindProperty("fillRatios");
        this.fillMethodProperty = this.serializedObject.FindProperty("m_FillMethod");
        this.fillAmountProperty = this.serializedObject.FindProperty("m_FillAmount");
        this.fillOriginProperty = this.serializedObject.FindProperty("m_FillOrigin");
        this.typeProperty = this.serializedObject.FindProperty("m_Type");
    }

    public override void OnInspectorGUI() {
        this.serializedObject.Update();
        EditorGUILayout.PropertyField(this.spriteProperty);
        this.typeProperty.enumValueIndex = 3;
        EditorGUI.BeginChangeCheck();
        var newValue = EditorGUILayout.IntSlider("Step Count", this.stepCountProperty.intValue, 0, this.maxStepCountProperty.intValue);
        if (EditorGUI.EndChangeCheck()) {
            this.stepCountProperty.intValue = newValue;
            this.fillAmountProperty.floatValue = (float)newValue / this.maxStepCountProperty.intValue;
        }
        EditorGUILayout.PropertyField(this.modeProperty);
        ShowFillParamsUI();
        if (((DiscreteGauge)this.target).Mode == DiscreteGauge.StepMode.Custom) {
            ShowFillRatiosUI();
        }
        this.serializedObject.ApplyModifiedProperties();
    }

    private void ShowFillParamsUI() {
        this.isFillParamsFolded = CustomEditorGUILayout.Foldout("Fill Params", this.isFillParamsFolded);
        if (this.isFillParamsFolded) {
            EditorGUILayout.PropertyField(this.maxStepCountProperty);
            EditorGUILayout.PropertyField(this.fillMethodProperty);
            EditorGUILayout.PropertyField(this.fillOriginProperty);
        }
    }

    private void ShowFillRatiosUI() {
        this.isFillRatiosFolded = CustomEditorGUILayout.Foldout("Fill Ratios", this.isFillRatiosFolded);
        if (this.isFillRatiosFolded) {
            this.fillRatiosProperty.arraySize = this.maxStepCountProperty.intValue + 1;
            for (int i = 0; i < this.maxStepCountProperty.intValue + 1; ++i) {
                var property = this.fillRatiosProperty.GetArrayElementAtIndex(i);
                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUILayout.Slider($"Step{i}", property.floatValue, 0.0f, 10.0f);
                if (EditorGUI.EndChangeCheck()) {
                    property.floatValue = newValue;
                }
            }
        }
    }
}
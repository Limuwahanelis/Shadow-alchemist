using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

[CustomEditor(typeof(ShadowRangeIndicator))]
[CanEditMultipleObjects]
public class ShadowRangeIndicatorEditor: Editor
{
    private SerializedProperty _indicator; 
    private SerializedProperty _isRightBorder;
    private SerializedProperty _shadow;
    private ShadowRangeIndicator _sri;
    private void OnEnable()
    {
        _indicator = serializedObject.FindProperty("_inGameIndicator");
        _isRightBorder = serializedObject.FindProperty("_isRightBorder");
        _shadow = serializedObject.FindProperty("_shadow");
        if (_indicator.objectReferenceValue == null || _shadow.objectReferenceValue == null) return;
        (_indicator.objectReferenceValue as GameObject).SetActive(true);
        _sri = (ShadowRangeIndicator)target;
    }
    public void TryAssign()
    {
        _indicator = serializedObject.FindProperty("_inGameIndicator");
        _isRightBorder = serializedObject.FindProperty("_isRightBorder");
        _shadow = serializedObject.FindProperty("_shadow");
        if (_indicator.objectReferenceValue == null || _shadow.objectReferenceValue == null) return;
        (_indicator.objectReferenceValue as GameObject).SetActive(true);
        _sri = (ShadowRangeIndicator)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (_indicator.objectReferenceValue == null || _shadow.objectReferenceValue==null || _sri ==null)
        {
            TryAssign();
            return;
        }
        if (_isRightBorder.boolValue) _sri.SetXPos((_shadow.objectReferenceValue as ControllableShadow).ShadowBounds.max.x + math.abs((_shadow.objectReferenceValue as ControllableShadow).transform.position.x - _sri.transform.position.x));
        else _sri.SetXPos((_shadow.objectReferenceValue as ControllableShadow).ShadowBounds.min.x - math.abs((_shadow.objectReferenceValue as ControllableShadow).transform.position.x - _sri.transform.position.x));
    }
    private void OnDisable()
    {
        if (_indicator.objectReferenceValue == null || _shadow.objectReferenceValue == null) return;
        (_indicator.objectReferenceValue as GameObject).SetActive(false);
    }
}
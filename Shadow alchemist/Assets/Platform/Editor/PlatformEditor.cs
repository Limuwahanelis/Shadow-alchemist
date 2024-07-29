using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Platform))]
public class PlatformEditor : Editor
{
    Platform _platform;
    SerializedProperty _platformX;
    SerializedProperty _platformY;
    SerializedProperty _renderer;
    SerializedProperty _collider;
    private void OnEnable()
    {
        _platform = (Platform)target;
        _platformX = serializedObject.FindProperty("platformX");
        _platformY = serializedObject.FindProperty("platformY");
        _renderer = serializedObject.FindProperty("spriteRend");
        _collider = serializedObject.FindProperty("col");
    }

    public override void OnInspectorGUI()
    {
        string[] exclude = new string[] { "platformX", "platformY" };
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_platformX);
        EditorGUILayout.PropertyField(_platformY);
        DrawPropertiesExcluding(serializedObject, exclude);
        if(EditorGUI.EndChangeCheck()) UpdateSprite();
        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateSprite()
    {
        if (_renderer.objectReferenceValue != null && _collider.objectReferenceValue != null)
        {
            Undo.RecordObject((_renderer.objectReferenceValue as SpriteRenderer), "renderer");
            Undo.RecordObject(_collider.objectReferenceValue as BoxCollider2D, "collider");
            (_renderer.objectReferenceValue as SpriteRenderer).size = new Vector2(_platformX.floatValue, _platformY.floatValue);
            (_collider.objectReferenceValue as BoxCollider2D).size = new Vector2((_renderer.objectReferenceValue as SpriteRenderer).size.x, _platformY.floatValue);
        }
    }
}

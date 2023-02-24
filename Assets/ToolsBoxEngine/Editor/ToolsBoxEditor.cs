using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace ToolsBoxEngine {
    public static class ToolsBoxEditor {
        public static void LogAllValues(SerializedProperty serializedProperty) {
            try {
                Debug.Log("PROPERTY: name = " + serializedProperty.name + " type = " + serializedProperty.type);
                Debug.Log("animationCurveValue = " + serializedProperty.animationCurveValue);
                Debug.Log("arraySize = " + serializedProperty.arraySize);
                Debug.Log("boolValue = " + serializedProperty.boolValue);
                Debug.Log("boundsValue = " + serializedProperty.boundsValue);
                Debug.Log("colorValue = " + serializedProperty.colorValue);
                Debug.Log("depth = " + serializedProperty.depth);
                Debug.Log("editable = " + serializedProperty.editable);
                Debug.Log("enumNames = " + serializedProperty.enumNames);
                Debug.Log("enumValueIndex = " + serializedProperty.enumValueIndex);
                Debug.Log("floatValue = " + serializedProperty.floatValue);
                Debug.Log("hasChildren = " + serializedProperty.hasChildren);
                Debug.Log("hasMultipleDifferentValues = " + serializedProperty.hasMultipleDifferentValues);
                Debug.Log("hasVisibleChildren = " + serializedProperty.hasVisibleChildren);
                Debug.Log("intValue = " + serializedProperty.intValue);
                Debug.Log("isAnimated = " + serializedProperty.isAnimated);
                Debug.Log("isArray = " + serializedProperty.isArray);
                Debug.Log("isExpanded = " + serializedProperty.isExpanded);
                Debug.Log("isInstantiatedPrefab = " + serializedProperty.isInstantiatedPrefab);
                Debug.Log("name = " + serializedProperty.name);
                Debug.Log("objectReferenceInstanceIDValue = " + serializedProperty.objectReferenceInstanceIDValue);
                Debug.Log("objectReferenceValue = " + serializedProperty.objectReferenceValue);
                Debug.Log("prefabOverride = " + serializedProperty.prefabOverride);
                Debug.Log("propertyPath = " + serializedProperty.propertyPath);
                Debug.Log("propertyType = " + serializedProperty.propertyType);
                Debug.Log("quaternionValue = " + serializedProperty.quaternionValue);
                Debug.Log("rectValue = " + serializedProperty.rectValue);
                Debug.Log("serializedObject = " + serializedProperty.serializedObject);
                Debug.Log("stringValue = " + serializedProperty.stringValue);
                Debug.Log("tooltip = " + serializedProperty.tooltip);
                Debug.Log("type = " + serializedProperty.type);
                Debug.Log("vector2Value = " + serializedProperty.vector2Value);
                Debug.Log("vector3Value = " + serializedProperty.vector3Value);
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }
    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}

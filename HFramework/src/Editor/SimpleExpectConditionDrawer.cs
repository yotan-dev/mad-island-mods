using UnityEngine;
using UnityEditor;
using HFramework.SexScripts.Info.NpcConditions;

/// <summary>
/// Custom drawer for conditions that are just "Expect" enum state.
/// It makes them an inline property simplifying the UI
/// </summary>
[CustomPropertyDrawer(typeof(Dead))]
[CustomPropertyDrawer(typeof(Faint))]
public class SimpleExpectConditionDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		var rect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent());

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(rect, property.FindPropertyRelative("Expected"), GUIContent.none);

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}

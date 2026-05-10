using System;
using System.Linq;
using HFramework.SexScripts.ScriptContext;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ContextTagAttribute))]
public class ContextTagDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType != SerializedPropertyType.String)
		{
			EditorGUI.LabelField(position, label.text, "Use ContextTag with a string field.");
			return;
		}

		var currentValue = property.stringValue ?? string.Empty;
		var known = ContextTagRegistry.GetKnownTagsSorted();

		var hasCustomValue = !string.IsNullOrWhiteSpace(currentValue) && !known.Contains(currentValue, StringComparer.OrdinalIgnoreCase);

		var choices = hasCustomValue
			? new[] { $"<custom: {currentValue}>" }.Concat(known).ToArray()
			: known;

		var selectedIndex = -1;
		if (hasCustomValue)
			selectedIndex = 0;
		else if (!string.IsNullOrWhiteSpace(currentValue))
			selectedIndex = Array.FindIndex(choices, c => string.Equals(c, currentValue, StringComparison.OrdinalIgnoreCase));

		EditorGUI.BeginProperty(position, label, property);

		var newIndex = EditorGUI.Popup(position, label.text, Mathf.Max(0, selectedIndex), choices);
		if (newIndex >= 0 && newIndex < choices.Length)
		{
			var chosen = choices[newIndex];
			if (!chosen.StartsWith("<custom:", StringComparison.Ordinal))
			{
				if (!string.Equals(property.stringValue, chosen, StringComparison.Ordinal))
				{
					property.stringValue = chosen;
					ContextTagRegistry.Register(chosen);
				}
			}
		}

		EditorGUI.EndProperty();
	}
}

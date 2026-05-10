using UnityEngine;
using UnityEditor;
using YotanModCore.Consts;
using System.Collections.Generic;
using System.Linq;

// @TODO: Move to YotanModCore
/// <summary>
/// Custom drawer for EmoteIdAttribute.
/// Should be used by adding `[EmoteId]` on an int field
/// </summary>
[CustomPropertyDrawer(typeof(EmoteIdAttribute))]
public class EmoteIdDrawer : PropertyDrawer
{
	private struct EmoteName {
		public readonly int EmoteId;
		public readonly string Name;

		public EmoteName(int id, string name) {
			this.EmoteId = id;
			this.Name = name;
		}
	}

	private static List<EmoteName> emoteNames = null;

	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		if (emoteNames == null) {
			emoteNames = new List<EmoteName>();

			var fields = typeof(Emote).GetFields();
			foreach (var field in fields) {
				var name = field.Name;
				var value = (int)field.GetValue(null);

				emoteNames.Add(new EmoteName(value, name));

				// if (field.GetCustomAttributes(typeof(StrValAttribute), false).FirstOrDefault() is StrValAttribute attr) {
				// 	NPCNames[value] = (string)attr.StrVal;
				// }
			}
		}

		if (property.propertyType != SerializedPropertyType.Integer) {
			EditorGUI.LabelField(position, label.text, "Use EmoteId with an int field.");
			return;
		}

		var options = emoteNames.Select(n => $"{n.Name} ({n.EmoteId})").ToArray();
		var selectedIndex = emoteNames.FindIndex(n => n.EmoteId == property.intValue);

		var currentValue = property.intValue;

		// Popup will also take the space of the label, so make it 75% of the size,
		// the rest goes to the int.
		var rect = position;
		rect.width *= 0.75f;

		var newIndex = EditorGUI.Popup(rect, label.text, selectedIndex, options);

		rect.x += rect.width;
		rect.width = position.width - rect.width;

		var intValue = EditorGUI.IntField(rect, property.intValue);
		property.intValue = intValue;

		if (selectedIndex != newIndex && newIndex >= 0 && newIndex < emoteNames.Count) {
			property.intValue = emoteNames[newIndex].EmoteId;
		} else if (intValue != currentValue) {
			property.intValue = intValue;
		}
	}
}

using UnityEngine;
using UnityEditor;
using YotanModCore.Consts;
using System.Collections.Generic;
using System.Linq;

// @TODO: Move to YotanModCore
/// <summary>
/// Custom drawer for NpcIdAttribute.
/// Should be used by adding `[NpcId]` on an int field
/// </summary>
[CustomPropertyDrawer(typeof(NpcIdAttribute))]
public class NpcIdDrawer : PropertyDrawer
{
	private struct NpcName {
		public readonly int NpcId;
		public readonly string Name;

		public NpcName(int id, string name) {
			this.NpcId = id;
			this.Name = name;
		}
	}

	private static List<NpcName> npcNames = null;

	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		if (npcNames == null) {
			npcNames = new List<NpcName>();

			var fields = typeof(NpcID).GetFields();
			foreach (var field in fields) {
				var name = field.Name;
				var value = (int)field.GetValue(null);

				npcNames.Add(new NpcName(value, name));

				// if (field.GetCustomAttributes(typeof(StrValAttribute), false).FirstOrDefault() is StrValAttribute attr) {
				// 	NPCNames[value] = (string)attr.StrVal;
				// }
			}
		}

		if (property.propertyType != SerializedPropertyType.Integer) {
			EditorGUI.LabelField(position, label.text, "Use NpcId with an int field.");
			return;
		}

		var options = npcNames.Select(n => $"{n.Name} ({n.NpcId})").ToArray();
		var selectedIndex = npcNames.FindIndex(n => n.NpcId == property.intValue);

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

		if (selectedIndex != newIndex && newIndex >= 0 && newIndex < npcNames.Count) {
			property.intValue = npcNames[newIndex].NpcId;
		} else if (intValue != currentValue) {
			property.intValue = intValue;
		}
	}
}

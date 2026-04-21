using System.Collections.Generic;
using System.Reflection;
using HFramework.ScriptNodes;
using HFramework.SexScripts;
using UnityEditor;
using UnityEngine;
using YotanModCore.Consts;

namespace HFramework.EditorUI
{
	[CustomPropertyDrawer(typeof(ActorIndexAttribute))]
	public class ActorIndexDrawer : PropertyDrawer
	{
		private static Dictionary<int, string> npcNameById;

		private static Dictionary<int, string> GetNpcNameMap()
		{
			if (npcNameById != null)
				return npcNameById;

			npcNameById = new Dictionary<int, string>();
			var fields = typeof(NpcID).GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (var field in fields)
			{
				if (field.FieldType != typeof(int))
					continue;

				var id = (int)field.GetValue(null);
				npcNameById[id] = field.Name;
			}

			return npcNameById;
		}

		private static SexScript TryGetOwningSexScript(SerializedObject serializedObject)
		{
			if (serializedObject == null)
				return null;

			if (serializedObject.targetObject is not ScriptNode node)
				return null;

			var assetPath = AssetDatabase.GetAssetPath(node);
			if (string.IsNullOrWhiteSpace(assetPath))
				return null;

			return AssetDatabase.LoadMainAssetAtPath(assetPath) as SexScript;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.LabelField(position, label.text, "Use ActorIndex with an int field.");
				return;
			}

			var sexScript = TryGetOwningSexScript(property.serializedObject);
			var npcs = sexScript?.Info?.Npcs;

			if (npcs == null || npcs.Length == 0)
			{
				EditorGUI.LabelField(position, label.text, "No actors found.");
				return;
			}

			var nameMap = GetNpcNameMap();

			var currentIndex = property.intValue;
			var hasInvalidValue = currentIndex < 0 || currentIndex >= npcs.Length;

			var choices = new string[npcs.Length + (hasInvalidValue ? 1 : 0)];
			var choiceOffset = hasInvalidValue ? 1 : 0;
			if (hasInvalidValue)
				choices[0] = $"<invalid: {currentIndex}>";

			for (var i = 0; i < npcs.Length; i++)
			{
				var npcId = npcs[i]?.NpcID ?? 0;
				var npcName = nameMap.TryGetValue(npcId, out var mapped) ? mapped : npcId.ToString();
				choices[i + choiceOffset] = $"{i} - {npcName} ({npcId})";
			}

			var selectedIndex = hasInvalidValue ? 0 : currentIndex;
			selectedIndex = Mathf.Clamp(selectedIndex, 0, choices.Length - 1);

			EditorGUI.BeginProperty(position, label, property);
			var newIndex = EditorGUI.Popup(position, label.text, selectedIndex, choices);
			if (newIndex != selectedIndex)
			{
				property.intValue = hasInvalidValue ? (newIndex - 1) : newIndex;
			}
			EditorGUI.EndProperty();
		}
	}
}

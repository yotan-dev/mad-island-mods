using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using HFramework.Tree;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace HFramework.Tree.EditorUI
{

	[CustomEditor(typeof(EmitEventNode))]
	public class EmitEventNode_Inspector : Editor
	{
		public VisualTreeAsset m_InspectorXML;

		public override VisualElement CreateInspectorGUI()
		{
			// Create a new VisualElement to be the root of our inspector UI
			VisualElement myInspector = new VisualElement();
			
			// Load from default reference
			m_InspectorXML.CloneTree(myInspector);

			var fields = TypeCache.GetFieldsWithAttribute<SexEventAttribute>();
			var choices = new List<string>();
			foreach (var fld in fields)
			{
				// var attr = fld.GetCustomAttribute<SexEventAttribute>();
				// choices.Add($"[{attr.Source}]{attr.Name}");

				// @TODO: Use the attribute source/name to better display. Internally map to id.
				// This can probably be done by creating a hidden field to bind to the object,
				// while keeping the dropdown as a UI-only element, which triggers RegisterValueChangedCallback

				// @TODO 2: Add a validation so we can warn about bad nodes.
				var id = (fld.GetValue(null) as SexEvent).id;
				choices.Add(id);
			}

			var vals = myInspector.Q<DropdownField>("Event_Dropdown");
			if (vals == null)
			{
				Debug.Log("Event_Dropdown not found");
			}

			// vals.RegisterValueChangedCallback((e) =>
			// {
			// 	Debug.Log("vals changed to " + e.newValue);
			// });
		
			vals.choices = choices;

			// Get a reference to the default inspector foldout control
			VisualElement inspectorFoldout = myInspector.Q("Default_Inspector");

			// Attach a default inspector to the foldout
			InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);

			// Return the finished inspector UI
			return myInspector;
		}

	}
}
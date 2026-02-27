using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using HFramework.Tree;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

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
				var id = (fld.GetValue(null) as IReadOnlySexEvent<SexEventArgs>).GetId();
				choices.Add(id);
			}

			var vals = myInspector.Q<DropdownField>("Event_Dropdown");
			if (vals == null)
			{
				Debug.Log("Event_Dropdown not found");
			}

			vals.choices = choices;

			// Get a reference to the default inspector foldout control
			VisualElement inspectorFoldout = myInspector.Q("Default_Inspector");

			// Attach a default inspector to the foldout
			InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);

			// Handle event type change so we can update the event args
			vals.RegisterValueChangedCallback((e) =>
			{
				var eventKeyRef = serializedObject.FindProperty("eventKey");
				if (eventKeyRef.stringValue == e.newValue || (eventKeyRef.stringValue == "" && e.newValue == null))
					return;

				// Unbind the inspector foldout from the serialized object
				// This is necessary because otherwise the changes we make to the serialized object
				// will not be persisted when the inspector redraws.
				inspectorFoldout.Unbind();

				// We need to manually handle the change callback, or it will lose the update
				eventKeyRef.stringValue = e.newValue;

				var eventArgsRef = serializedObject.FindProperty("EventArgs");
				if (SexEvents.Events.TryGetValue(e.newValue, out var eventInfo))
				{
					try
					{
						var eventArgs = Activator.CreateInstance(eventInfo.EventType);
						eventArgsRef.managedReferenceValue = eventArgs;
						serializedObject.ApplyModifiedProperties();
						serializedObject.Update();
					}
					catch (Exception ex)
					{
						Debug.LogError($"Error triggering event {e.newValue}: {ex}");
					}
				}
				inspectorFoldout.Bind(serializedObject);
			});

			// Return the finished inspector UI
			return myInspector;
		}

	}
}

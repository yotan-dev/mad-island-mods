using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using HFramework.ScriptNodes;
using HFramework.Events;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace HFramework.EditorUI.SexScripts
{

	[CustomEditor(typeof(EmitEvent))]
	public class EmitEventNode_Inspector : Editor
	{
		public VisualTreeAsset m_InspectorXML;
		public VisualTreeAsset m_EventTemplateXML;

		private List<string> SexEventsList = new();

		private void LoadSexEventsList() {
			var fields = TypeCache.GetFieldsWithAttribute<SexEventAttribute>();
			var choices = new List<string>();
			foreach (var fld in fields) {
				// var attr = fld.GetCustomAttribute<SexEventAttribute>();
				// choices.Add($"[{attr.Source}]{attr.Name}");

				// @TODO: Use the attribute source/name to better display. Internally map to id.
				// This can probably be done by creating a hidden field to bind to the object,
				// while keeping the dropdown as a UI-only element, which triggers RegisterValueChangedCallback

				// @TODO 2: Add a validation so we can warn about bad nodes.
				var id = (fld.GetValue(null) as IReadOnlySexEvent<SexEventArgs>).GetId();
				choices.Add(id);
			}

			this.SexEventsList = choices;
		}

		private void SetupEventDropdown_Old(DropdownField eventDropdown, VisualElement eventFoldout, SerializedProperty eventKeyRef, SerializedProperty eventArgsRef) {
			eventDropdown.choices = this.SexEventsList;

			// Handle event type change so we can update the event args
			eventDropdown.RegisterValueChangedCallback((e) => {
				if (eventKeyRef.stringValue == e.newValue || (eventKeyRef.stringValue == "" && e.newValue == null))
					return;

				// Unbind the inspector foldout from the serialized object
				// This is necessary because otherwise the changes we make to the serialized object
				// will not be persisted when the inspector redraws.
				eventFoldout.Unbind();

				// We need to manually handle the change callback, or it will lose the update
				eventKeyRef.stringValue = e.newValue;

				if (SexEvents.Events.TryGetValue(e.newValue, out var eventInfo)) {
					try {
						var eventArgs = Activator.CreateInstance(eventInfo.EventType);
						eventArgsRef.managedReferenceValue = eventArgs;
						serializedObject.ApplyModifiedProperties();
						serializedObject.Update();
					} catch (Exception ex) {
						Debug.LogError($"Error triggering event {e.newValue}: {ex}");
					}
				}
				eventFoldout.Bind(serializedObject);
			});
		}

		private void SetupEventDropdown(DropdownField eventDropdown, SerializedProperty eventKeyRef) {
			eventDropdown.choices = this.SexEventsList;
			eventDropdown.bindingPath = eventKeyRef.propertyPath;
			eventDropdown.value = eventKeyRef.stringValue;
		}

		private EventCallback<ChangeEvent<string>> BuildEventDropwdownChangedHandler(DropdownField eventDropdown, PropertyField propField) {
			return (ChangeEvent<string> e) => {
				var newValue = e.newValue;

				if (string.IsNullOrEmpty(newValue))
					return;

				// Unbind the inspector propField from the serialized object
				// This is necessary because otherwise the changes we make to the serialized object
				// will not be persisted when the inspector redraws.
				propField.Unbind();

				var pathParts = eventDropdown.bindingPath.Split('.');
				var eventPath = string.Join(".", pathParts.Take(pathParts.Length - 1));
				var eventKeyRef = serializedObject.FindProperty($"{eventPath}.{nameof(EmitEvent.EventEntry.EventKey)}");
				var eventArgsRef = serializedObject.FindProperty($"{eventPath}.{nameof(EmitEvent.EventEntry.EventArgs)}");

				// We need to manually handle the change callback, or it will lose the update
				eventKeyRef.stringValue = e.newValue;

				if (SexEvents.Events.TryGetValue(e.newValue, out var eventInfo)) {
					try {
						var eventArgs = Activator.CreateInstance(eventInfo.EventType);
						eventArgsRef.managedReferenceValue = eventArgs;
						serializedObject.ApplyModifiedProperties();
						serializedObject.Update();
					} catch (Exception ex) {
						Debug.LogError($"Error triggering event {e.newValue}: {ex}");
					}
				}

				// Rebinds the propField to the serialized object
				propField.Bind(serializedObject);
			};
		}

		private void SetupFoldout(VisualElement foldout) {
			// Attach a default inspector to the foldout
			InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
		}

		private void SetupEventsList(ListView listView) {
			// Get the target EmitEvent component
			var emitEvent = target as EmitEvent;

			// 1. Assign the data source
			listView.itemsSource = emitEvent.Events;

			// 2. Set how to create a new row (using your template)
			listView.makeItem = () => {
				var element = m_EventTemplateXML.CloneTree();

				var eventDropdown = element.Q<DropdownField>("Event_Dropdown");
				var properties = element.Q<PropertyField>("Properties");
				eventDropdown.RegisterValueChangedCallback(BuildEventDropwdownChangedHandler(eventDropdown, properties));

				return element;
			};

			var eventsArray = serializedObject.FindProperty("Events");

			// 3. Initialize new items when added
			listView.itemsAdded += (indices) => {
				foreach (var i in indices) {
					if (emitEvent.Events[i] == null) {
						emitEvent.Events[i] = new EmitEvent.EventEntry();
					}
					eventsArray.InsertArrayElementAtIndex(i);
				}
			};

			listView.itemsRemoved += (indices) => {
				foreach (var i in indices) {
					eventsArray.DeleteArrayElementAtIndex(i);
				}
			};

			// 4. Set how to bind data to that row
			listView.bindItem = (element, i) => {
				var entry = emitEvent.Events[i];

				// Bind the event dropdown
				var eventDropdown = element.Q<DropdownField>("Event_Dropdown");
				var propField = element.Q<PropertyField>("Properties");

				var eventKeyRef = eventsArray.GetArrayElementAtIndex(i).FindPropertyRelative("EventKey");
				var eventArgsRef = eventsArray.GetArrayElementAtIndex(i).FindPropertyRelative("EventArgs");

				// Bind the property field
				propField.bindingPath = eventArgsRef.propertyPath;
				if (eventArgsRef.managedReferenceValue != null) {
					propField.Bind(serializedObject);
				}

				this.SetupEventDropdown(eventDropdown, eventKeyRef);
			};
		}

		public override VisualElement CreateInspectorGUI() {
			// Create a new VisualElement to be the root of our inspector UI
			var myInspector = new VisualElement();

			// Create the base UI inside it
			m_InspectorXML.CloneTree(myInspector);
			if (serializedObject.targetObject == null) {
				this.ResetTarget();
				return myInspector;
			}

			// Get references to each element
			var oldEventDropdown = myInspector.Q<DropdownField>("Event_Dropdown");
			var oldEventFoldout = myInspector.Q("Default_Inspector");
			var listView = myInspector.Q<ListView>("Events_List");

			// Load the SexEvents available to the engine
			this.LoadSexEventsList();

			// Setup the old components
			this.SetupEventDropdown_Old(
				oldEventDropdown,
				oldEventFoldout,
				serializedObject.FindProperty(nameof(EmitEvent.EventKey)),
				serializedObject.FindProperty(nameof(EmitEvent.EventArgs))
			);
			this.SetupFoldout(oldEventFoldout);

			// Setup the new component
			this.SetupEventsList(listView);

			// Return the finished inspector
			return myInspector;
		}
	}
}

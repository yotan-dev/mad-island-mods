using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace HFramework.Tree.EditorUI
{
	public class InspectorView : VisualElement
	{
		public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

		Editor editor;

		public InspectorView()
		{

		}

		internal void UpdateSelection(NodeView nodeView)
		{
			Clear();

			UnityEngine.Object.DestroyImmediate(editor); // destroy previous editor

			if (nodeView.node is EmitEventNode emitEventNode) {
				Debug.Log("Emit event");
				editor = EmitEventNode_Inspector.CreateEditor(nodeView.node);
				var container = editor.CreateInspectorGUI();
				var so = new SerializedObject(nodeView.node);
				container.Bind(so);
				// var container = new IMGUIContainer(() =>
				// {
				// 	editor.OnInspectorGUI();
				// });
				Add(container);
			} else {
				Debug.Log("Other");
				editor = Editor.CreateEditor(nodeView.node);
				var container = new IMGUIContainer(() =>
				{
					editor.OnInspectorGUI();
				});
				Add(container);
			}

		}
	}
}

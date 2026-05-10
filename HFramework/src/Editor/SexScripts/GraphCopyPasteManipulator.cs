// Not sure why, but dotnet thinks these should be public (seems like it is looking at some publicized assembly)
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using HFramework.ScriptNodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HFramework.EditorUI.SexScripts
{
	public class GraphCopyPasteManipulator : Manipulator
	{
		private const string ClipboardPrefix = "SexScriptNodes";

		public class ClipboardNodeData
		{
			public string TypeName { get; set; }
			public Vector2 OriginalPosition { get; set; }
			public string JsonData { get; set; }

			public string ToClipboard() {
				return $"{this.TypeName}|{this.OriginalPosition.x}|{this.OriginalPosition.y}|{this.JsonData}";
			}

			public static ClipboardNodeData From(string data) {
				var parts = data.Split('|');
				if (parts.Length < 4) {
					throw new ArgumentException("Invalid clipboard data format");
				}

				var result = new ClipboardNodeData();
				result.TypeName = parts[0];
				result.OriginalPosition = new Vector2(float.Parse(parts[1]), float.Parse(parts[2]));
				result.JsonData = parts[3];
				return result;
			}
		}

		private Vector2 LastLocalMousePosition;

		protected override void RegisterCallbacksOnTarget() {
			if (target is not SexScriptView) {
				Debug.LogError("GraphCopyPasteManipulator should only be added to a SexScriptView!");
				return;
			}

			this.target.RegisterCallback<KeyDownEvent>(OnKeyDown);
			this.target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
		}

		protected override void UnregisterCallbacksFromTarget() {
			this.target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
			this.target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
		}

		private void OnMouseMove(MouseMoveEvent evt) {
			// Track mouse position so we know where to paste
			LastLocalMousePosition = evt.localMousePosition;
		}

		private void OnKeyDown(KeyDownEvent evt) {
			// Handle Ctrl+C for copy
			if (evt.ctrlKey && evt.keyCode == KeyCode.C) {
				CopySelectedNodes();
				evt.StopPropagation();
			}
			// Handle Ctrl+V for paste
			else if (evt.ctrlKey && evt.keyCode == KeyCode.V) {
				PasteNodes();
				evt.StopPropagation();
			}
		}

		private void CopySelectedNodes() {
			var graphView = target as SexScriptView;

			var selectedNodes = graphView.selection.OfType<NodeView>().ToList();
			if (selectedNodes.Count == 0) {
				return;
			}

			var clipboardData = new List<ClipboardNodeData>();
			foreach (var nodeView in selectedNodes) {
				var node = nodeView.node;
				node = node.Clone(null);

				// Clear GUID and remove connections, since those are not copy/pasteable
				node.GUID = string.Empty;
				node.ClearChildren();

				// Store node type, position, and serialized data
				clipboardData.Add(new ClipboardNodeData {
					TypeName = node.GetType().AssemblyQualifiedName,
					OriginalPosition = node.Position,
					JsonData = JsonUtility.ToJson(node)
				});
			}

			var clipboardContent = $"{ClipboardPrefix}\n{string.Join("\n", clipboardData.Select(c => c.ToClipboard()))}";
			EditorGUIUtility.systemCopyBuffer = clipboardContent;
		}

		private void PasteNodes() {
			var systemClipboard = EditorGUIUtility.systemCopyBuffer;
			if (string.IsNullOrEmpty(systemClipboard) || !systemClipboard.StartsWith(ClipboardPrefix)) {
				return;
			}

			var graphView = target as SexScriptView;

			var clipboardNodes = systemClipboard
				.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Skip(1) // Removes the prefix line
				.Select(line => ClipboardNodeData.From(line))
				.ToArray();
			var pastePosition = (Vector2)graphView.viewTransform.matrix.inverse.MultiplyPoint(this.LastLocalMousePosition);
			var nodesCenter = this.GetCopiedNodesCenter(clipboardNodes);

			foreach (var data in clipboardNodes) {
				var typeName = data.TypeName;
				var originalX = data.OriginalPosition.x;
				var originalY = data.OriginalPosition.y;
				var jsonData = data.JsonData;

				try {
					var nodeType = Type.GetType(typeName);
					if (nodeType != null) {
						ScriptNode newNode = null; // It will be assigned on the line below, but set to null so editor doesn't complain
#if UNITY_EDITOR
						newNode = graphView.tree.CreateNode(nodeType);
#endif
						JsonUtility.FromJsonOverwrite(jsonData, newNode);

						// Place node at cursor position, maintaining relative positions between multiple nodes
						var relativeOffset = new Vector2(originalX, originalY) - nodesCenter;
						newNode.Position = pastePosition + relativeOffset;

						// Generate new GUID to avoid conflicts
						newNode.GUID = System.Guid.NewGuid().ToString();

						(target as SexScriptView).CreateNodeView(newNode, true);
					}
				} catch (System.Exception e) {
					Debug.LogError($"Failed to paste node: {e.Message}");
				}
			}
		}

		private Vector2 GetCopiedNodesCenter(ClipboardNodeData[] clipboardNodes) {
			if (clipboardNodes == null || clipboardNodes.Length == 0) {
				return Vector2.zero;
			}

			var bounds = new Rect();
			var firstNode = true;

			foreach (var data in clipboardNodes) {
				var originalX = data.OriginalPosition.x;
				var originalY = data.OriginalPosition.y;
				var nodePos = new Rect(originalX, originalY, 100, 100); // Default node size

				if (firstNode) {
					bounds = nodePos;
					firstNode = false;
				} else {
					bounds.xMin = Mathf.Min(bounds.xMin, nodePos.xMin);
					bounds.yMin = Mathf.Min(bounds.yMin, nodePos.yMin);
					bounds.xMax = Mathf.Max(bounds.xMax, nodePos.xMax);
					bounds.yMax = Mathf.Max(bounds.yMax, nodePos.yMax);
				}
			}

			return bounds.center;
		}
	}
}
#endif

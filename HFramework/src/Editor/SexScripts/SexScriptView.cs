using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using HFramework.ScriptNodes;
using UnityEditor;
using System;
using System.Linq;
using HFramework.SexScripts;
using UnityEngine;
using System.Reflection;

namespace HFramework.EditorUI.SexScripts
{
	public class SexScriptView : GraphView
	{
		public Action<NodeView> OnNodeSelected;

		public new class UxmlFactory : UxmlFactory<SexScriptView, GraphView.UxmlTraits> { }

		SexScript tree;

		public SexScriptView()
		{
			NodeEvents.OnNodeIDChanged += OnNodeIDChanged;

			Insert(0, new GridBackground());

			this.AddManipulator(new ContentZoomer());
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.yotan-dev.hframework/Editor/SexScripts/SexScriptEditor.uss");
			styleSheets.Add(styleSheet);
		}

		private void OnNodeIDChanged(ScriptNode node) {
			var nodeView = FindNodeView(node);
			if (nodeView == null) {
				return;
			}

			// NOTE: "node" itself will be included in the list
			var nodesWithID = this.tree.Nodes.FindAll(otherNode => otherNode.ID == node.ID);
			if (nodesWithID.Count > 1) {
				Debug.LogWarning($"Multiple nodes with ID \"{node.ID}\" found in the SexScript \"{tree.name}\". IDs should be unique within the SexScript.");
			}
		}

		NodeView FindNodeView(ScriptNode node)
		{
			return GetNodeByGuid(node.GUID) as NodeView;
		}

		Port GetOutputPort(NodeView parentView, ScriptNode childNode)
		{
			var root = parentView.node as Root;
			if (root != null && parentView.teardownOutput != null && root.TeardownNode == childNode)
			{
				return parentView.teardownOutput;
			}

			return parentView.output;
		}

		internal void PopulateView(SexScript tree)
		{
			this.tree = tree;

			graphViewChanged -= OnGraphViewChanged;
			DeleteElements(graphElements);
			graphViewChanged += OnGraphViewChanged;

			if (tree.RootNode == null)
			{
#if UNITY_EDITOR
				tree.RootNode = tree.CreateNode(typeof(Root)) as Root;
				EditorUtility.SetDirty(tree);
				AssetDatabase.SaveAssets();
#endif
			}

			// Creates nodes views
			tree.Nodes.ForEach(n => CreateNodeView(n));

			// Creates edges
			tree.Nodes.ForEach(n =>
			{
				var children = tree.GetChildren(n);
				children.ForEach(c =>
				{
					var parentView = FindNodeView(n);
					var childView = FindNodeView(c);

					var outputPort = GetOutputPort(parentView, c);
					var edge = outputPort.ConnectTo(childView.input);
					AddElement(edge);
				});
			});
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
		}

		private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
		{
			if (graphViewChange.elementsToRemove != null)
			{
				graphViewChange.elementsToRemove.ForEach(elem =>
				{
					var nodeView = elem as NodeView;
					if (nodeView != null)
					{
#if UNITY_EDITOR
						tree.DeleteNode(nodeView.node);
#endif
					}

					var edge = elem as Edge;
					if (edge != null)
					{
						var parentView = edge.output.node as NodeView;
						var childView = edge.input.node as NodeView;
						tree.RemoveChild(parentView.node, childView.node, edge.output.portName);
					}
				});
			}

			if (graphViewChange.edgesToCreate != null)
			{
				graphViewChange.edgesToCreate.ForEach(edge =>
				{
					var parentView = edge.output.node as NodeView;
					var childView = edge.input.node as NodeView;

					tree.AddChild(parentView.node, childView.node, edge.output.portName);
				});
			}
			return graphViewChange;
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			var position = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);

			// base.BuildContextualMenu(evt);
			var types = TypeCache.GetTypesWithAttribute<ScriptNodeAttribute>();
			foreach (var type in types)
			{
				var attr = type.GetCustomAttribute<ScriptNodeAttribute>();
				if (attr != null)
				{
					var nameParts = attr.MenuName.Split('/');
					nameParts[^1] = $"[{attr.Source}] {nameParts[^1]}";
					evt.menu.AppendAction(string.Join('/', nameParts), (a) => CreateNode(type, position));
				}
			}
		}

		void CreateNode(System.Type type, Vector2 position)
		{
#if UNITY_EDITOR
			var node = tree.CreateNode(type);
			node.Position = position;
			CreateNodeView(node);
#endif
		}


		void CreateNodeView(ScriptNode node)
		{
			var nodeView = new NodeView(node);
			nodeView.OnNodeSelected = OnNodeSelected;
			AddElement(nodeView);
		}
	}
}

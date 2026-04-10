using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace HFramework.Tree.EditorUI
{
	public class BehaviourTreeView : GraphView
	{
		public Action<NodeView> OnNodeSelected;

		public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

		BehaviourTree tree;

		public BehaviourTreeView()
		{
			Insert(0, new GridBackground());

			this.AddManipulator(new ContentZoomer());
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.yotan-dev.hframework/Editor/Editor/BehaviourTreeEditor.uss");
			styleSheets.Add(styleSheet);
		}

		NodeView FindNodeView(Node node)
		{
			return GetNodeByGuid(node.GUID) as NodeView;
		}

		Port GetOutputPort(NodeView parentView, Node childNode)
		{
			var root = parentView.node as RootNode;
			if (root != null && parentView.teardownOutput != null && root.teardownNode == childNode)
			{
				return parentView.teardownOutput;
			}

			return parentView.output;
		}

		internal void PopulateView(BehaviourTree tree)
		{
			this.tree = tree;

			graphViewChanged -= OnGraphViewChanged;
			DeleteElements(graphElements);
			graphViewChanged += OnGraphViewChanged;

			if (tree.rootNode == null)
			{
#if UNITY_EDITOR
				tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
				EditorUtility.SetDirty(tree);
				AssetDatabase.SaveAssets();
#endif
			}

			// Creates nodes views
			tree.nodes.ForEach(n => CreateNodeView(n));

			// Creates edges
			tree.nodes.ForEach(n =>
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
			// base.BuildContextualMenu(evt);
			var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
			foreach (var type in types)
			{
				evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
			}

			types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
			foreach (var type in types)
			{
				evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
			}

			types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
			foreach (var type in types)
			{
				evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
			}
		}

		void CreateNode(System.Type type)
		{
#if UNITY_EDITOR
			var node = tree.CreateNode(type);
			CreateNodeView(node);
#endif
		}


		void CreateNodeView(Node node)
		{
			var nodeView = new NodeView(node);
			nodeView.OnNodeSelected = OnNodeSelected;
			AddElement(nodeView);
		}
	}
}

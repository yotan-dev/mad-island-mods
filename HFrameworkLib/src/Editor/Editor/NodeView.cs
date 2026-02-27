using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace HFramework.Tree.EditorUI
{
	public class NodeView : UnityEditor.Experimental.GraphView.Node
	{
		public Action<NodeView> OnNodeSelected;

		public Node node;

		public Port input;

		public Port output;

		public Port teardownOutput;

		public NodeView(Node node)
		{
			this.node = node;
			this.title = node.name;
			this.viewDataKey = node.GUID; // MEtadata for GraphView

			style.left = node.position.x;
			style.top = node.position.y;

			CreateInputPorts();
			CreateOutputPorts();
		}

		private void CreateInputPorts()
		{
			if (node is ActionNode)
			{
				input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
			}
			else if (node is CompositeNode)
			{
				input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
			}
			else if (node is DecoratorNode)
			{
				input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
			}
			else if (node is RootNode)
			{

			}

			if (input != null)
			{
				input.portName = "";
				inputContainer.Add(input);
			}
		}

		private void CreateOutputPorts()
		{
			if (node is ActionNode)
			{
				// no ouputs
			}
			else if (node is CompositeNode)
			{
				output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
			}
			else if (node is DecoratorNode)
			{
				output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
			}
			else if (node is DecoratorNode)
			{
				output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
			}
			else if (node is RootNode)
			{
				output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
				output.portName = "";

				teardownOutput = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
				teardownOutput.portName = BehaviourTree.TeardownPortName;
			}

			if (output != null)
			{
				outputContainer.Add(output);
			}

			if (teardownOutput != null)
			{
				outputContainer.Add(teardownOutput);
			}
		}

		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			node.position.x = newPos.xMin;
			node.position.y = newPos.yMin;
		}

		public override void OnSelected()
		{
			base.OnSelected();
			if (OnNodeSelected != null)
			{
				OnNodeSelected.Invoke(this);
			}
		}
	}
}

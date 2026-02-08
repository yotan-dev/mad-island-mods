using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
	public Node rootNode;

	public Node.State treeState = Node.State.Running;

	public List<Node> nodes = new List<Node>();

	public Node.State Update()
	{
		if (rootNode.state == Node.State.Running)
		{
			treeState = rootNode.Update();
		}

		return treeState;
	}

	public Node CreateNode(System.Type type)
	{
		var node = ScriptableObject.CreateInstance(type) as Node;
		node.name = type.Name;
		node.GUID = GUID.Generate().ToString();
		nodes.Add(node);

		AssetDatabase.AddObjectToAsset(node, this);
		AssetDatabase.SaveAssets();
		return node;
	}

	public void DeleteNode(Node node)
	{
		nodes.Remove(node);
		AssetDatabase.RemoveObjectFromAsset(node);
		AssetDatabase.SaveAssets();
	}

	public void AddChild(Node parent, Node child)
	{
		var decorator = parent as DecoratorNode;
		if (decorator)
		{
			decorator.child = child;
		}

		var root = parent as RootNode;
		if (root)
		{
			root.child = child;
		}

		var composite = parent as CompositeNode;
		if (composite)
		{
			composite.children.Add(child);
		}
	}

	public void RemoveChild(Node parent, Node child)
	{
		var decorator = parent as DecoratorNode;
		if (decorator)
		{
			decorator.child = null;
		}

		var root = parent as RootNode;
		if (root)
		{
			root.child = null;
		}

		var composite = parent as CompositeNode;
		if (composite)
		{
			composite.children.Remove(child);
		}
	}

	public List<Node> GetChildren(Node parent)
	{
		var children = new List<Node>();

		var decorator = parent as DecoratorNode;
		if (decorator && decorator.child != null)
		{
			children.Add(decorator.child);
		}

		var root = parent as RootNode;
		if (root && root.child != null)
		{
			children.Add(root.child);
		}

		var composite = parent as CompositeNode;
		if (composite)
		{
			return composite.children;
		}

		return children;
	}

	public BehaviourTree Clone()
	{
		var tree = Instantiate(this);
		tree.rootNode = tree.rootNode.Clone();
		return tree;
	}
}

using System.Collections.Generic;
using HFramework.SexScripts.Info;
using HFramework.ScriptNodes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	[Experimental]
	public class SexScript : ScriptableObject
	{
		/// <summary>
		/// Sex script engine version.
		/// This will be used if HFramework needs to do drastic changes to its own Nodes, which will lead to a new engine version but allow backward compatibility.
		/// </summary>
		[Tooltip("Sex script engine version.\nThis will be used if HFramework needs to do drastic changes to its own Nodes, which will lead to a new engine version but allow backward compatibility.")]
		public EngineVersion EngineVersion = EngineVersion.V1;

		/// <summary>
		/// The Unique Identifier of this SexScript, it may be used if we want to reference it from code/other mods.
		/// Should be unique across all mods.
		///
		/// Recommended format: "modname.scriptname"
		/// </summary>
		[Tooltip("The Unique Identifier of this SexScript, it may be used if we want to reference it from code/other mods.\nShould be unique across all mods.\nRecommended format: \"modname.scriptname\"")]
		public string UniqueID;

		/// <summary>
		/// The display name of the sex script.
		/// May be shown to players in the UI.
		/// </summary>
		[Tooltip("The display name of the sex script.\nMay be shown to players in the UI.")]
		public string Name;

		/// <summary>
		/// The description of the sex script.
		/// </summary>
		[TextArea(3, 10)]
		public string Description;

		public SexScriptInfo Info;

		public static string TeardownPortName = "Teardown";

		[HideInInspector]
		public ScriptNode rootNode;

		[HideInInspector]
		public ScriptNode.State treeState = ScriptNode.State.Running;

		public CommonContext context;

		[HideInInspector]
		public List<ScriptNode> nodes = new List<ScriptNode>();

		public ScriptNode.State Update() {
			if (rootNode.state == ScriptNode.State.Running) {
				treeState = rootNode.Update();
			}

			return treeState;
		}

#if UNITY_EDITOR
		private string GenerateNodeId(System.Type type)
		{
			var newId = type.Name.ToString();
			var idCount = 0;
			bool isUnique = false;
			while (!isUnique)
			{
				isUnique = true;
				foreach (var node in nodes)
				{
					if (node.ID == newId)
					{
						isUnique = false;
						break;
					}
				}
				if (!isUnique)
				{
					idCount++;
					newId = $"{type.Name}_{idCount}";
				}
			}

			return newId;
		}

		public ScriptNode CreateNode(System.Type type)
		{
			var node = ScriptableObject.CreateInstance(type) as ScriptNode;
			node.name = type.Name;
			node.GUID = GUID.Generate().ToString();
			node.ID = this.GenerateNodeId(type);
			nodes.Add(node);

			AssetDatabase.AddObjectToAsset(node, this);
			AssetDatabase.SaveAssets();
			return node;
		}

		public void DeleteNode(ScriptNode node)
		{
			nodes.Remove(node);
			AssetDatabase.RemoveObjectFromAsset(node);
			AssetDatabase.SaveAssets();
		}
#endif

		public void AddChild(ScriptNode parent, ScriptNode child, string portName = "") {
			var decorator = parent as Passthrough;
			if (decorator) {
				decorator.child = child;
			}

			var root = parent as Root;
			if (root) {
				if (portName == TeardownPortName) {
					root.teardownNode = child;
				} else {
					root.child = child;
				}
			}

			var composite = parent as Composite;
			if (composite) {
				composite.children.Add(child);
			}
		}

		public void RemoveChild(ScriptNode parent, ScriptNode child, string portName = "") {
			var decorator = parent as Passthrough;
			if (decorator) {
				decorator.child = null;
			}

			var root = parent as Root;
			if (root) {
				if (portName == TeardownPortName) {
					root.teardownNode = null;
				} else {
					root.child = null;
				}
			}

			var composite = parent as Composite;
			if (composite) {
				composite.children.Remove(child);
			}
		}

		public List<ScriptNode> GetChildren(ScriptNode parent) {
			var children = new List<ScriptNode>();

			var decorator = parent as Passthrough;
			if (decorator && decorator.child != null) {
				children.Add(decorator.child);
			}

			var root = parent as Root;
			if (root && root.child != null) {
				children.Add(root.child);
			}

			if (root && root.teardownNode != null) {
				children.Add(root.teardownNode);
			}

			var composite = parent as Composite;
			if (composite) {
				return composite.children;
			}

			return children;
		}

		internal SexScript Clone() {
			var tree = Instantiate(this);
			tree.context = new CommonContext(this);
			tree.rootNode = tree.rootNode.Clone(tree.context);
			return tree;
		}
	}
}


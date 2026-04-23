using System.Collections.Generic;
using HFramework.SexScripts.Info;
using HFramework.ScriptNodes;
using UnityEngine;
using HFramework.SexScripts.ScriptContext;
using UnityEditor;
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

		[Tooltip("Minimum game version required to run this script. Format: xx.xx.xx.xx (e.g. 0.5.9.0; if parts are omitted, they are assumed as 0)")]
		public string MinimalGameVersion = "0.0.0.0";

		[Tooltip("Whether this script requires the game DLC to be installed.")]
		public bool RequiresDLC = false;

		public SexScriptInfo Info;

		public static string TeardownPortName = "Teardown";

		[HideInInspector]
		public ScriptNode RootNode;

		[HideInInspector]
		public ScriptNode.State TreeState = ScriptNode.State.Running;

		public CommonContext Context;

		[HideInInspector]
		public List<ScriptNode> Nodes = new List<ScriptNode>();

		public ScriptNode.State Update() {
			if (RootNode.ScriptState == ScriptNode.State.Running) {
				TreeState = RootNode.Update();
			}

			return TreeState;
		}

#if UNITY_EDITOR
		private string GenerateNodeId(System.Type type) {
			var newId = type.Name.ToString();
			var idCount = 0;
			bool isUnique = false;
			while (!isUnique) {
				isUnique = true;
				foreach (var node in this.Nodes) {
					if (node.ID == newId) {
						isUnique = false;
						break;
					}
				}
				if (!isUnique) {
					idCount++;
					newId = $"{type.Name}_{idCount}";
				}
			}

			return newId;
		}

		public ScriptNode CreateNode(System.Type type) {
			var node = ScriptableObject.CreateInstance(type) as ScriptNode;
			node.name = type.Name;
			node.GUID = GUID.Generate().ToString();
			node.ID = this.GenerateNodeId(type);

			Undo.RecordObject(this, "SexScript (CreateNode)");
			this.Nodes.Add(node);

			AssetDatabase.AddObjectToAsset(node, this);
			Undo.RegisterCreatedObjectUndo(node, "SexScript (CreateNode)");

			AssetDatabase.SaveAssets();
			return node;
		}

		public void DeleteNode(ScriptNode node) {
			Undo.RecordObject(this, "SexScript (DeleteNode)");
			this.Nodes.Remove(node);

			// AssetDatabase.RemoveObjectFromAsset(node);
			Undo.DestroyObjectImmediate(node);

			AssetDatabase.SaveAssets();
		}
#endif

		public void AddChild(ScriptNode parent, ScriptNode child, string portName = "") {
			var decorator = parent as Passthrough;
			if (decorator) {
				Undo.RecordObject(decorator, "SexScript (AddChild)");
				decorator.Child = child;
				EditorUtility.SetDirty(decorator);
			}

			var root = parent as Root;
			if (root) {
				Undo.RecordObject(root, "SexScript (AddChild)");
				if (portName == TeardownPortName) {
					root.TeardownNode = child;
				} else {
					root.Child = child;
				}
				EditorUtility.SetDirty(root);
			}

			var composite = parent as Composite;
			if (composite) {
				Undo.RecordObject(composite, "SexScript (AddChild)");
				composite.Children.Add(child);
				EditorUtility.SetDirty(composite);
			}
		}

		public void RemoveChild(ScriptNode parent, ScriptNode child, string portName = "") {
			var decorator = parent as Passthrough;
			if (decorator) {
				Undo.RecordObject(decorator, "SexScript (RemoveChild)");
				decorator.Child = null;
				EditorUtility.SetDirty(decorator);
			}

			var root = parent as Root;
			if (root) {
				Undo.RecordObject(root, "SexScript (RemoveChild)");
				if (portName == TeardownPortName) {
					root.TeardownNode = null;
				} else {
					root.Child = null;
				}
				EditorUtility.SetDirty(root);
			}

			var composite = parent as Composite;
			if (composite) {
				Undo.RecordObject(composite, "SexScript (RemoveChild)");
				composite.Children.Remove(child);
				EditorUtility.SetDirty(composite);
			}
		}

		public List<ScriptNode> GetChildren(ScriptNode parent) {
			var children = new List<ScriptNode>();

			var decorator = parent as Passthrough;
			if (decorator && decorator.Child != null) {
				children.Add(decorator.Child);
			}

			var root = parent as Root;
			if (root && root.Child != null) {
				children.Add(root.Child);
			}

			if (root && root.TeardownNode != null) {
				children.Add(root.TeardownNode);
			}

			var composite = parent as Composite;
			if (composite) {
				return composite.Children;
			}

			return children;
		}

		internal SexScript Clone() {
			var tree = Instantiate(this);
			tree.Context = new CommonContext(this);
			tree.RootNode = tree.RootNode.Clone(tree.Context);
			return tree;
		}
	}
}


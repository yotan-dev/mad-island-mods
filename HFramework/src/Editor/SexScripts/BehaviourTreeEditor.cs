using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using HFramework.ScriptNodes;
using HFramework.SexScripts;

namespace HFramework.EditorUI.SexScripts
{
	public class BehaviourTreeEditor : EditorWindow
	{
		BehaviourTreeView treeView;
		InspectorView inspectorView;


		[MenuItem("Behaviour Tree/Editor...")]
		public static void OpenWindow()
		{
			BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
			wnd.titleContent = new GUIContent("BehaviourTreeEditor");
		}

		public void CreateGUI()
		{
			// Each editor window contains a root VisualElement object
			VisualElement root = rootVisualElement;

			// Import UXML
			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.yotan-dev.hframework/Editor/SexScripts/BehaviourTreeEditor.uxml");
			visualTree.CloneTree(root);

			// A stylesheet can be added to a VisualElement.
			// The style will be applied to the VisualElement and all of its children.
			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.yotan-dev.hframework/Editor/SexScripts/BehaviourTreeEditor.uss");
			root.styleSheets.Add(styleSheet);

			treeView = root.Q<BehaviourTreeView>();
			inspectorView = root.Q<InspectorView>();
			treeView.OnNodeSelected = OnNodeSelectionChanged;

			OnSelectionChange(); // Trigger recreation after editing/opening the UI
		}

		public void OnSelectionChange()
		{
			var tree = Selection.activeObject as SexScript;
			if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
			{
				treeView.PopulateView(tree);
			}
		}

		void OnNodeSelectionChanged(NodeView node)
		{
			inspectorView.UpdateSelection(node);
		}
	}
}

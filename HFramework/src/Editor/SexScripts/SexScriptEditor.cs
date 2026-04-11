using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using HFramework.SexScripts;

namespace HFramework.EditorUI.SexScripts
{
	public class SexScriptEditor : EditorWindow
	{
		SexScriptView treeView;
		InspectorView inspectorView;


		[MenuItem("Sex Script/Editor...")]
		public static void OpenWindow()
		{
			SexScriptEditor wnd = GetWindow<SexScriptEditor>();
			wnd.titleContent = new GUIContent("SexScriptEditor");
		}

		public void CreateGUI()
		{
			// Each editor window contains a root VisualElement object
			VisualElement root = rootVisualElement;

			// Import UXML
			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.yotan-dev.hframework/Editor/SexScripts/SexScriptEditor.uxml");
			visualTree.CloneTree(root);

			// A stylesheet can be added to a VisualElement.
			// The style will be applied to the VisualElement and all of its children.
			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.yotan-dev.hframework/Editor/SexScripts/SexScriptEditor.uss");
			root.styleSheets.Add(styleSheet);

			treeView = root.Q<SexScriptView>();
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

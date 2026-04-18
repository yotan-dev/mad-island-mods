using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using HFramework.ScriptGenerator;

namespace HFramework.EditorUI
{
	public static class RunSexScriptGenerators
	{
		private static List<ISexScriptGenerator> FromGUIDs(string[] guids) {
			var generators = new List<ISexScriptGenerator>();
			foreach (var guid in guids) {
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
				var type = script.GetClass();
				if (type != null && typeof(ISexScriptGenerator).IsAssignableFrom(type)) {
					var generator = type.GetConstructor(Type.EmptyTypes)?.Invoke(null) as ISexScriptGenerator;
					generators.Add(generator);
				}
			}
			return generators;
		}

		private static List<ISexScriptGenerator> FromFolder(string folderPath) {
			var scriptGUIDs = AssetDatabase.FindAssets("t:MonoScript", new[] { folderPath });
			return FromGUIDs(scriptGUIDs);
		}

		[MenuItem("Assets/HFramework Tools/Run script generators")]
		private static void GenerateFromDataAction() {
			Debug.Log("Genering Sex Scripts...");

			var selectedObjects = Selection.assetGUIDs;
			if (selectedObjects.Length == 0) {
				return;
			}

			List<ISexScriptGenerator> generators = new List<ISexScriptGenerator>();
			if (selectedObjects.Length == 1 && AssetDatabase.IsValidFolder(AssetDatabase.GUIDToAssetPath(selectedObjects[0]))) {
				var folderPath = AssetDatabase.GUIDToAssetPath(selectedObjects[0]);
				generators = FromFolder(folderPath);
			} else {
				generators = FromGUIDs(selectedObjects);
			}

			foreach (var generator in generators) {
				generator.Generate();
			}

			AssetDatabase.SaveAssets();
			Debug.Log("Generated Sex Scripts!");
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Serialization;
using HarmonyLib;
using UnityEngine.Assertions;
using YotanModCore.DataStore;

namespace YotanModCore.Patches
{
	[HarmonyPatch]
	internal static class TranspileSaveFileSerializer
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			var saveCoroutineMethod = AccessTools.Method(typeof(SaveManager), nameof(SaveManager.SaveCoroutine));
			var loadStartMethod = AccessTools.Method(typeof(SaveManager), nameof(SaveManager.LoadStart));

			var result = new List<MethodBase>
			{
				AccessTools.EnumeratorMoveNext(saveCoroutineMethod),
				AccessTools.EnumeratorMoveNext(loadStartMethod)
			};

			return result;
		}

		private static XmlSerializer GetSerializer(Type type)
		{
			if (type == typeof(SaveManager.SaveDataBase))
			{
				return new XmlSerializer(type, DataStoreManager.GetSaveDataTypes());
			}

			return new XmlSerializer(type);
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			var codeMatcher = new CodeMatcher(instructions, generator);

			/**
			 * Looks for:
			 	new XmlSerializer(typeof(x));

			 * Replace with:
			 	TranspileSaveFileSerializer.GetSerializer(typeof(x));
			 */

			int hitCount = 0;
			while (true)
			{
				bool found = false;

				codeMatcher
					.Start()
					.MatchForward(
						false, // beginning
						new CodeMatch(OpCodes.Newobj, AccessTools.Constructor(typeof(XmlSerializer), new Type[] { typeof(Type) }))
					);

				found = !codeMatcher.IsInvalid;
				if (!found)
					break;

				codeMatcher.RemoveInstruction();
				codeMatcher.InsertAndAdvance(
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspileSaveFileSerializer), nameof(TranspileSaveFileSerializer.GetSerializer)))
				);

				hitCount++;
			}

			Assert.IsTrue(hitCount > 0, "Could not find any XmlSerializer instances");

			return codeMatcher.Instructions();
		}
	}
}

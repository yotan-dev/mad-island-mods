using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Serialization;
using HarmonyLib;
using UnityEngine.Assertions;

namespace YotanModCore.DataStore
{
	[HarmonyPatch]
	internal static class TranspileSaveFileSerializer
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			var result = new List<MethodBase>();

			if (GameInfo.GameVersion >= GameInfo.ToVersion("0.6.0.0"))
			{
				// v0.6.0 moved XmlSerializer construction out of the save/load coroutines
				// into cached static properties. Patch the property getters instead.
				result.Add(AccessTools.Method(typeof(SaveManager), "get_NativeSaveDataSerializer", Type.EmptyTypes));
				result.Add(AccessTools.Method(typeof(SaveManager), "get_NativeSaveViewSerializer", Type.EmptyTypes));
			}
			else
			{
				var saveCoroutineMethod = AccessTools.Method(typeof(SaveManager), nameof(SaveManager.SaveCoroutine));
				var loadStartMethod = AccessTools.Method(typeof(SaveManager), nameof(SaveManager.LoadStart));

				result.Add(AccessTools.EnumeratorMoveNext(saveCoroutineMethod));
				result.Add(AccessTools.EnumeratorMoveNext(loadStartMethod));
			}

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

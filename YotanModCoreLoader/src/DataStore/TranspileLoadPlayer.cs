using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace YotanModCore.DataStore
{
	[HarmonyPatch]
	internal static class TranspileLoadPlayer
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			var loadBuildCoroutineMethod = AccessTools.Method(typeof(SaveManager), nameof(SaveManager.LoadBuild), []);

			var result = new List<MethodBase>
			{
				AccessTools.EnumeratorMoveNext(loadBuildCoroutineMethod),
			};

			return result;
		}

		private static void LoadPlayerData(SaveManager __instance)
		{
			// SaveManager::TempToSave builds 2 arrays for that:
			// - pc (playerCommons)
			// - pl (playerSave)
			//
			// We are just repeating them here because making use of the loop is too hard.
			for (int i = 0; i < __instance.saveDB.save[0].playerSave.Length; i++)
				SaveManagerPatch.LoadCommonData(__instance.gameMN.playerCommons[i], __instance.saveDB.save[0].playerSave[i]);
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			var codeMatcher = new CodeMatcher(instructions, generator);

			/**
			 * Looks for: (Currently, it only exists one instance of it, at the end of the player data saving)
			 	TreasureScript[] componentsInChildren = GameObject.Find("StaticBG").transform.Find("Treasure").GetComponentsInChildren<TreasureScript>(true);

			 * Insert before:
			 	TranspileLoadPlayer.SavePlayerData(this);
			 */

			codeMatcher
				.Start()
				.MatchForward(
					false, // beginning
					new CodeMatch(OpCodes.Ldstr, "StaticBG")
				)
				.ThrowIfNotMatch("Could not find StaticBG load");

			codeMatcher.InsertAndAdvance(
				new CodeInstruction(OpCodes.Ldloc_1),
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspileLoadPlayer), nameof(TranspileLoadPlayer.LoadPlayerData)))
			);

			return codeMatcher.Instructions();
		}
	}
}

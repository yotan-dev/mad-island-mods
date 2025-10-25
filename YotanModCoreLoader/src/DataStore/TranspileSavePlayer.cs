using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace YotanModCore.DataStore
{
	/// <summary>
	/// Patches the saving process to include saving of Player data,
	/// which doesn't have a dedicated function for us to hook into.
	/// </summary>
	[HarmonyPatch]
	internal static class TranspileSavePlayer
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			var tempToSaveCoroutineMethod = AccessTools.Method(typeof(SaveManager), nameof(SaveManager.TempToSave));

			var result = new List<MethodBase>
			{
				AccessTools.EnumeratorMoveNext(tempToSaveCoroutineMethod),
			};

			return result;
		}

		private static void SavePlayerData(SaveManager __instance)
		{
			// SaveManager::TempToSave builds 2 arrays for that:
			// - commonStatesArray (playerCommons)
			// - charaSaveArray (playerSave)
			//
			// We are just repeating them here because making use of the loop is too hard.
			for (int i = 0; i < __instance.saveEntry.playerSave.Length; i++)
				SaveCharDataPatch.SaveCommonData(__instance.gameMN.playerCommons[i], __instance.saveEntry.playerSave[i]);
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			var codeMatcher = new CodeMatcher(instructions, generator);

			/**
			 * Looks for: (Currently, it only exists one instance of it, at the end of the player data saving)
			 	this.saveEntry.baseSave.collectSave = new int[inventory.collections.Length];

			 * Insert after:
			 	TranspileSavePlayer.SavePlayerData(this);
			 */

			codeMatcher
				.Start()
				.MatchForward(
					true, // end
					new CodeMatch(OpCodes.Newarr, typeof(System.Int32)),
					new CodeMatch(OpCodes.Stfld, typeof(SaveManager.BaseSave).GetField("collectSave"))
				)
				.ThrowIfNotMatch("Could not find any collectSave array creation");

			codeMatcher.InsertAndAdvance(
				new CodeInstruction(OpCodes.Ldloc_1),
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspileSavePlayer), nameof(TranspileSavePlayer.SavePlayerData)))
			);

			return codeMatcher.Instructions();
		}
	}
}

using HarmonyLib;
using MadIsland.FreeMode;
using UnityEngine;

namespace HFrameworkLoader.Patches
{
	/// <summary>
	/// Extends the Free Mode co-op interaction replication (v0.6.0+) with
	/// family 4 ("mod prefab registry") so mod-spawned sex visuals can be
	/// resolved and validated on remote clients. Logic lives in HFramework.
	/// </summary>
	public class FreeModePatches
	{
		[HarmonyPatch(typeof(FreeInteractionPrefabBinding), nameof(FreeInteractionPrefabBinding.TryResolve))]
		[HarmonyPostfix]
		private static void Post_TryResolve(
			SexManager manager,
			int family,
			int list,
			int index,
			ref GameObject prefab,
			ref string signature,
			ref bool __result)
		{
			if (__result)
				return;

			__result = Plugin.Bridge.FreeMode_TryResolvePrefab(manager, family, list, index, out prefab, out signature);
		}

		[HarmonyPatch(typeof(FreeInteractionVisual), nameof(FreeInteractionVisual.Valid))]
		[HarmonyPostfix]
		private static void Post_VisualValid(FreeInteractionVisual value, ref bool __result)
		{
			if (__result)
				return;

			__result = Plugin.Bridge.FreeMode_ValidateVisual(value);
		}
	}
}

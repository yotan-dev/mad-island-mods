#nullable enable

using System;
using HarmonyLib;

namespace HFramework.Patches
{
	public class NpcMovePatches
	{
		public static event EventHandler<NPCMove.ActType>? OnActTypeChanged;

		[HarmonyPatch(typeof(NPCMove), nameof(NPCMove.Wait))]
		[HarmonyPrefix]
		private static void NPCMove_Wait(NPCMove __instance)
		{
			OnActTypeChanged?.Invoke(__instance, __instance.actType);
		}
	}
}

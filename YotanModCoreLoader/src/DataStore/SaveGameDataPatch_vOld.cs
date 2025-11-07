using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace YotanModCore.DataStore
{
	/// <summary>
	/// Patches SaveManager for SaveManager that only works on versions < v0.4.5.6
	/// It simply proxies the calls to SaveGameDataPatch_Common
	/// </summary>
	public class SaveGameDataPatch_vOld
	{
		/// <summary>
		/// Patches the Loading process to load custom data
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.LoadBuild), [])]
		[HarmonyPrefix]
		private static void Pre_SaveManager_LoadBuild(SaveManager __instance)
		{
			SaveGameDataPatch_Common.Pre_SaveManager_Loading(__instance);
		}
	}
}

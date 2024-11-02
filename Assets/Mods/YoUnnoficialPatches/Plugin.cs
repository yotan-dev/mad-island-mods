using BepInEx;
using HarmonyLib;
using UnityEngine;
using YoUnnoficialPatches.Patches;

namespace YoUnnoficialPatches
{
	[BepInPlugin("YoUnnoficialPatches", "YoUnnoficialPatches", "0.2.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static ManagersScript ManagerScript;

		private void Awake()
		{
			PLogger._Logger = Logger;

			Harmony.CreateAndPatchAll(typeof(TranslationPatch));

			PLogger.LogInfo($"Plugin YoUnnoficialPatches is loaded!");
		}
	}
}

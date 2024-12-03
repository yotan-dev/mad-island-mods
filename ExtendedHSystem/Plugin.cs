using BepInEx;
using ExtendedHSystem.Patches;
using HarmonyLib;
using UnityEngine;

namespace ExtendedHSystem
{
	
	[BepInPlugin("ExtendedHSystem", "ExtendedHSystem", "0.1.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static ManagersScript ManagerScript;

		private void Awake()
		{
			PLogger._Logger = Logger;

			Harmony.CreateAndPatchAll(typeof(SexManager_CommonSexNPCPatch));
			Harmony.CreateAndPatchAll(typeof(SexManager_CommonSexPlayerPatch));
			Harmony.CreateAndPatchAll(typeof(SexManager_PlayerRapedPatch));

			PLogger.LogInfo($"Plugin ExtendedHSystem is loaded!");
		}
	}
}

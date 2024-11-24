using BepInEx;
using HarmonyLib;
using NpcStats.Patches;
using UnityEngine;

namespace NpcStats
{
	
	[BepInPlugin("NpcStats", "NpcStats", "2.0.1")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static ManagersScript ManagerScript;

		private void Awake()
		{
			PLogger._Logger = Logger;

			Harmony.CreateAndPatchAll(typeof(NpcSpawnPatch));

			PLogger.LogInfo($"Plugin NpcStats is loaded!");
		}
	}
}

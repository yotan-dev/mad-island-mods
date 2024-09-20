using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace EnhanceWorkplaces
{
	[BepInPlugin("EnhanceWorkplaces", "EnhanceWorkplaces", "0.1.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static ManagersScript ManagerScript;

		private void Awake()
		{
			EnhanceWorkplaces.Config.Instance.Init(Config);

			PLogger._Logger = Logger;
			
			Harmony.CreateAndPatchAll(typeof(WorkResultPatch));

			PLogger.LogInfo($"Plugin Enhance Workplaces is loaded!");
		}
	}
}

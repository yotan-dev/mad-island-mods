using BepInEx;
using HarmonyLib;
using YotanModCore.Patches;
using UnityEngine;

namespace YotanModCore
{
	
	[BepInPlugin("YotanModCore", "YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static ManagersScript ManagerScript;

		private void Awake()
		{
			PLogger._Logger = Logger;
			PLogger.LogInfo($"> Game Version: {GameInfo.ToVersionString(GameInfo.GameVersion)}");
			PLogger.LogInfo($">> DLC: {GameInfo.HasDLC}");

			CommonUtils.Init();

			Harmony.CreateAndPatchAll(typeof(ManagersPatch));

			PLogger.LogInfo($"Plugin YotanModCore is loaded!");
		}
	}
}

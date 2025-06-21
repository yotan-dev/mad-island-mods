using BepInEx;
using HarmonyLib;
using YotanModCore.Patches;
using UnityEngine;
using YotanModCore.Events;
using YotanModCore.Console;
using YotanModCore.NpcTalk;
using YotanModCore.Items;
using YotanModCore.Items.Patches;

namespace YotanModCore
{
	
	[BepInPlugin("YotanModCore", "YotanModCore", "2.0.2")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static ManagersScript ManagerScript;

		private void Awake()
		{
			PLogger._Logger = Logger;
			PLogger.LogInfo($"> Game Version: {GameInfo.ToVersionString(GameInfo.GameVersion)}");
			PLogger.LogInfo($">> DLC: {GameInfo.HasDLC}");
			PLogger.LogInfo($">> Censor Type: {GameInfo.CensorType}");

			Assets = AssetBundle.LoadFromFile($"BepInEx/plugins/YotanModCore/YotanModCore.assets");

			CommonUtils.Init();
			ConsoleManager.Instance.Init();
			NpcTalkManager.Init();

			Harmony.CreateAndPatchAll(typeof(ItemManagerPatches));
			Harmony.CreateAndPatchAll(typeof(DebugToolPatch));
			Harmony.CreateAndPatchAll(typeof(ManagersPatch));
			Harmony.CreateAndPatchAll(typeof(PropPanelsPatch));
			Harmony.CreateAndPatchAll(typeof(GameLifecycleEvents));

			ItemDB.Init();
			BundleLoader.Load();
			ItemDB.Instance.PostProcessItems();

			PLogger.LogInfo($"Plugin YotanModCore is loaded!");
		}
	}
}

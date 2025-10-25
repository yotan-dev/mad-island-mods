using BepInEx;
using HarmonyLib;
using YotanModCore.Patches;
using UnityEngine;
using YotanModCore.NpcTalk;
using YotanModCore.Items;
using YotanModCore.Items.Patches;
using YotanModCoreLoader.Patches;

namespace YotanModCore
{
	[BepInPlugin("YotanModCore", "YotanModCore", "2.1.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static ManagersScript ManagerScript;

		internal static InitializerResult InitializerResult;

		private void Awake()
		{
			PLogger._Logger = Logger;
			InitializerResult = Initializer.Init(new BepisLogger(Logger));

			Assets = AssetBundle.LoadFromFile($"BepInEx/plugins/YotanModCore/YotanModCore.assets");

			NpcTalkManager.Init();

			Harmony.CreateAndPatchAll(typeof(CraftManagerPatches));
			Harmony.CreateAndPatchAll(typeof(InventoryManagerPatches));
			Harmony.CreateAndPatchAll(typeof(ItemManagerPatches));
			Harmony.CreateAndPatchAll(typeof(DebugToolPatch));
			Harmony.CreateAndPatchAll(typeof(ManagersPatch));
			Harmony.CreateAndPatchAll(typeof(PropPanelsPatch));
			Harmony.CreateAndPatchAll(typeof(LifecyclePatch));

			Harmony.CreateAndPatchAll(typeof(TranspileDefenceInfo));
			Harmony.CreateAndPatchAll(typeof(DefenceInfoPatch));

			PLogger.LogInfo($"Plugin YotanModCore is loaded!");
		}

		private void Start()
		{
			// This must be delayed until Start or it might be too early for some stuff to be registered
			BundleLoader.Load();
			CraftDB.Instance.LoadRecipes(Managers.mn.craftMN);
		}
	}
}

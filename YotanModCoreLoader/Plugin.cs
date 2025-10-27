using BepInEx;
using HarmonyLib;
using YotanModCore.Patches;
using UnityEngine;
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

		internal static ModCoreBridge ModCoreBridge;

		private void Awake()
		{
			PLogger._Logger = Logger;
			ModCoreBridge = Initializer.Init(new BepisLogger(Logger));

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

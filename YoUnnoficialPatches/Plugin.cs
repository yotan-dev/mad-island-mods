using BepInEx;
using HarmonyLib;
using YoUnnoficialPatches.Patches;

namespace YoUnnoficialPatches
{
	[BepInPlugin("YoUnnoficialPatches", "YoUnnoficialPatches", "0.4.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = Logger;
			
			PConfig.Instance.Init(Config);

			Harmony.CreateAndPatchAll(typeof(TranslationPatch));

			if (PConfig.Instance.DontStartInvalidSex.Value)
				Harmony.CreateAndPatchAll(typeof(DontStartInvalidSexPatch));

			if (PConfig.Instance.FixMosaic.Value)
				Harmony.CreateAndPatchAll(typeof(FixMosaicPatch));

			PLogger.LogInfo($"Plugin YoUnnoficialPatches is loaded!");
		}
	}
}

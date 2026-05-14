using BepInEx;
using HarmonyLib;
using YoUnnoficialPatches.Patches;

namespace YoUnnoficialPatches
{
	[BepInPlugin("YoUnnoficialPatches", "YoUnnoficialPatches", "0.5.0")]
	[BepInDependency("YotanModCore", "2.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake() {
			PLogger._Logger = Logger;

			PConfig.Instance.Init(Config);

			if (PConfig.Instance.FixTranslationToEnglish.Value)
				Harmony.CreateAndPatchAll(typeof(TranslationPatch));

			if (PConfig.Instance.DontStartInvalidSex.Value)
				Harmony.CreateAndPatchAll(typeof(DontStartInvalidSexPatch));

			PLogger.LogInfo($"Plugin YoUnnoficialPatches is loaded!");
		}

		private void Start() {
			// FixMosaic should be delayed to Start so it gives time to Managers to get loaded
			if (PConfig.Instance.FixMosaic.Value)
				FixMosaicPatch.Apply();
		}
	}
}

using BepInEx;
using HarmonyLib;
using YotanModCore;
using YoUnnoficialPatches.Patches;
using YoUnnoficialPatches.Patches.Compat;

namespace YoUnnoficialPatches
{
	[BepInPlugin("YoUnnoficialPatches", "YoUnnoficialPatches", "0.5.1")]
	[BepInDependency("YotanModCore", "2.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake() {
			PLogger._Logger = Logger;

			PConfig.Instance.Init(Config);

			if (PConfig.Instance.FixTranslationToEnglish.Value)
				Harmony.CreateAndPatchAll(typeof(TranslationPatch));

			if (PConfig.Instance.DontStartInvalidSex.Value) {
				if (GameInfo.GameVersion >= GameInfo.ToVersion(0, 5, 12))
					Harmony.CreateAndPatchAll(typeof(DontStartInvalidSexPatch));
				else // < 0.5.12 ; compatibility.
					Harmony.CreateAndPatchAll(typeof(DontStartInvalidSexPatch_Pre0_5_12));
			}

			PLogger.LogInfo($"Plugin YoUnnoficialPatches is loaded!");
		}

		private void Start() {
			// FixMosaic should be delayed to Start so it gives time to Managers to get loaded
			if (PConfig.Instance.FixMosaic.Value)
				FixMosaicPatch.Apply();
		}
	}
}

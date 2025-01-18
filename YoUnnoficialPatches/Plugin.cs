using BepInEx;
using HarmonyLib;
using YoUnnoficialPatches.Patches;

namespace YoUnnoficialPatches
{
	[BepInPlugin("YoUnnoficialPatches", "YoUnnoficialPatches", "0.3.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = Logger;
			
			YoUnnoficialPatches.Config.Instance.Init(Config);

			Harmony.CreateAndPatchAll(typeof(TranslationPatch));

			if (YoUnnoficialPatches.Config.Instance.DontStartInvalidSex.Value)
				Harmony.CreateAndPatchAll(typeof(DontStartInvalidSexPatch));

			PLogger.LogInfo($"Plugin YoUnnoficialPatches is loaded!");
		}
	}
}

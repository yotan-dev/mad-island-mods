using BepInEx;
using HarmonyLib;

namespace StackNearby
{
	
	[BepInPlugin("StackNearby", "StackNearby", "1.0.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = Logger;

			Harmony.CreateAndPatchAll(typeof(BuildInfoPatch));
			Harmony.CreateAndPatchAll(typeof(PlayerMovePatch));

			Logger.LogInfo($"Plugin StackNearby is loaded!");
		}
	}
}

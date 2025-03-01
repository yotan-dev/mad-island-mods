using BepInEx;
using HarmonyLib;

namespace ItemSlotColor
{
	
	[BepInPlugin("ItemSlotColor", "ItemSlotColor", "1.0.2")]
	[BepInDependency("YotanModCore", BepInDependency.DependencyFlags.HardDependency)]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = Logger;
			Harmony.CreateAndPatchAll(typeof(ItemSlotPatch));

			Logger.LogInfo($"Plugin ItemSlotColor is loaded!");
		}
	}
}

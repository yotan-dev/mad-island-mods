using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace ItemSlotColor
{
	
	[BepInPlugin("ItemSlotColor", "ItemSlotColor", "1.0.1")]
	[BepInDependency("YotanModCore", BepInDependency.DependencyFlags.HardDependency)]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			Harmony.CreateAndPatchAll(typeof(ItemSlotPatch));

			Logger.LogInfo($"Plugin ItemSlotColor is loaded!");
		}
	}
}

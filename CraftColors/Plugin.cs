using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace CraftColors
{
	
	[BepInPlugin("CraftColors", "CraftColors", "1.0.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static ManagersScript ManagerScript;

		private void Awake()
		{
			Harmony.CreateAndPatchAll(typeof(ItemDescriptionPatch));

			Logger.LogInfo($"Plugin CraftColors is loaded!");
		}
	}
}

using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace Base
{
	
	[BepInPlugin("Base", "Base", "0.1.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static ManagersScript ManagerScript;

		private void Awake()
		{
			PLogger._Logger = Logger;

			// Harmony.CreateAndPatchAll(typeof(ManagersPatch));

			PLogger.LogInfo($"Plugin Base is loaded!");
		}
	}
}

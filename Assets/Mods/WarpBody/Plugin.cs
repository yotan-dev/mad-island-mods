using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace WarpBody
{
	[BepInPlugin("WarpBody", "WarpBody", "1.0.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			Harmony.CreateAndPatchAll(typeof(PlayerMovePatch));

			Logger.LogInfo($"Plugin WarpBody is loaded!");
		}
	}
}

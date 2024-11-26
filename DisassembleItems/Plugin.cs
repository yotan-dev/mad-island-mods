using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace DisassembleItems
{	
	[BepInPlugin("DisassembleItems", "DisassembleItems", "1.0.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = Logger;

			Harmony.CreateAndPatchAll(typeof(DisassembleItemsPatch));
			DisassembleTable.Init();

			PLogger.LogInfo($"Plugin Disassemble Items is loaded!");
		}
	}
}

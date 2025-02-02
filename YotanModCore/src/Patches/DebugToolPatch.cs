using HarmonyLib;
using UnityEngine.UI;
using YotanModCore.Console;

namespace YotanModCore.Patches
{
	internal class DebugToolPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(DebugTool), nameof(DebugTool.DebugCodeEnter))]
		internal static bool Pre_DebugTool_DebugCodeEnter(string code, InputField ___debugInput)
		{
			var text = code;
			if (text == "")
				text = ___debugInput.text;

			if (!ConsoleManager.Instance.Handle(text))
				return true;

			___debugInput.text = "";
			return false;
		}
	}
}

using System;
using HarmonyLib;

namespace YotanModCore.Console.Commands
{
	/// <summary>
	/// Forces stun damage to [value]. 0 disables it.
	/// 
	/// Syntax: /stun [value]
	/// </summary>
	public class StunCmd : ConsoleCmd
	{
		private static bool Patched = false;

		private static int StunDamage = 0;

		private static void Patch()
		{
			if (Patched)
				return;

			Harmony.CreateAndPatchAll(typeof(StunCmd));
			Patched = true;
		}

		[HarmonyPrefix, HarmonyPatch(typeof(CommonStates), nameof(CommonStates.StunDamage))]
		private static void Pre_CommonStates_StunDamage(CommonStates attacker, ref float damageRate)
		{
			if (StunDamage == 0 || attacker != CommonUtils.GetActivePlayer())
				return;

			damageRate = StunDamage;
		}

		public StunCmd()
		{
			Patch();
		}

		public override void Execute(string command, string[] arguments)
		{
			if (arguments.Length == 0)
				return;

			Int32.TryParse(arguments[0], out StunDamage);
		}
	}
}

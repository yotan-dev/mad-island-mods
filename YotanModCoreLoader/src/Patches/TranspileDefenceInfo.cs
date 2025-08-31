using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using YotanModCore.Items;

namespace YotanModCore.Patches
{
	[HarmonyPatch]
	internal static class TranspileDefenceInfo
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			var result = new List<MethodBase>
			{
				AccessTools.Method(typeof(DefenceInfo), nameof(DefenceInfo.DefenceDamage))
			};
			return result;
		}

		private static bool DefenceCounterAttack(DefenceInfo __instance, CommonStates attacker, float fixedDamage = 0f)
		{
			PLogger.LogDebug($"DefenceCounterAttack({attacker}, {fixedDamage})");
			if (__instance is CDefenceInfo cDefenceInfo)
				return cDefenceInfo.DefenceCounterAttack(attacker, fixedDamage);

			return false;
		}

		private static void Inject_DefenceCounterAttack(CodeMatcher codeMatcher)
		{
			/*
			 * Looks for:
				default:
					if (this.attack > 0.0) {

			* Replace with:
				default:
					if (TranspileDefenceInfo.DefenceCounterAttack()) {
					}
			*/
			codeMatcher
				.Start()
				.MatchForward(
					false, // beginning
					new CodeMatch(OpCodes.Ldarg_0), // this
					new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(DefenceInfo), nameof(DefenceInfo.attack))), // attack
					new CodeMatch(OpCodes.Ldc_R4, 0.0f), // 0.0f
					new CodeMatch(OpCodes.Ble_Un) // decompiler shows Ble_Un_S, but it is actually Ble_Un
				)
				.ThrowIfInvalid("Could not find default counter check: if (this.attack > 0.0)");

			var blockStartPos = codeMatcher.Pos;

			codeMatcher.Advance(3);
			var jumpAddress = codeMatcher.Instruction.operand;

			// We don't replace the first opcode (Ldarg_0), because it has labels
			// for the default branching of the switch.
			// Since it is the same we want in our static, we leave it there.
			codeMatcher
				.Start()
				.Advance(blockStartPos + 1)
				.RemoveInstructions(3)
				.InsertAndAdvance(
					// new CodeInstruction(OpCodes.Ldarg_0), // this
					new CodeInstruction(OpCodes.Ldarg_1), // attacker
					new CodeInstruction(OpCodes.Ldarg_2), // fixedDamage
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspileDefenceInfo), nameof(TranspileDefenceInfo.DefenceCounterAttack))),
					new CodeInstruction(OpCodes.Brtrue, jumpAddress)
				);
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			var codeMatcher = new CodeMatcher(instructions, generator);

			Inject_DefenceCounterAttack(codeMatcher);

			return codeMatcher.Instructions();
		}
	}
}

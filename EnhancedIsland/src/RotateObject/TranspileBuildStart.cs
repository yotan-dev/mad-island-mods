using HarmonyLib;
using UnityEngine;
using YotanModCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;
using YotanModCore.Items;

namespace EnhancedIsland.RotateObject
{
	[HarmonyPatch]
	internal class TranspileBuildStart
	{
		private static readonly MethodBase buildStartEnumerator = AccessTools.Method(typeof(BuildManager), nameof(BuildManager.BuildStart));
		private static readonly MethodInfo buildStartMoveNext = AccessTools.EnumeratorMoveNext(buildStartEnumerator);

		private static IEnumerable<MethodBase> TargetMethods()
		{
			return new List<MethodBase>
			{
				buildStartMoveNext,
			};
		}

		private static void RotateCheck(GameObject tmpBuild)
		{
			RotateUtils.RotateCheck(tmpBuild);
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			var codeMatcher = new CodeMatcher(instructions, generator);

			/*
			 * Looks for:
				while (!buildEnd && buildManager.nowBuild)
				{
					if (Physics.Raycast( ....

			* Replace with:
				while (!buildEnd && buildManager.nowBuild)
				{
					TranspileBuildStart.RotateCheck(); // <---- Insert
					if (Physics.Raycast( ....

			*/

			// IL_038c: br           IL_0a27

			// IL_0391: call         class [UnityEngine.CoreModule]UnityEngine.Camera [UnityEngine.CoreModule]UnityEngine.Camera::get_main()

			// ....

			//  IL_03f4: call         bool [UnityEngine.PhysicsModule]UnityEngine.Physics::Raycast(valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.CoreModule]UnityEngine.Vector3, valuetype [UnityEngine.PhysicsModule]UnityEngine.RaycastHit&, float32, int32)
			codeMatcher
				.Start()
				.MatchForward(
					false, // beginning
					new CodeMatch((CodeInstruction instruction) => {
						if (instruction.opcode != OpCodes.Call)
							return false;

						var methodInfo = (System.Reflection.MethodInfo)instruction.operand;
						return methodInfo.DeclaringType.Name == "Physics" && methodInfo.Name == "Raycast";
					})
				)
				.ThrowIfInvalid("Could not find Physics.Raycast call");

			codeMatcher
				.MatchBack(
					false, // beginning
					new CodeMatch((CodeInstruction instruction) => {
						if (instruction.opcode != OpCodes.Call)
							return false;

						var methodInfo = (System.Reflection.MethodInfo)instruction.operand;
						return methodInfo.DeclaringType.Name == "Camera" && methodInfo.Name == "get_main";
					})
				)
				.ThrowIfInvalid("Could not find Camera.main call");

			codeMatcher
				.MatchBack(
					true, // end of instruction
					new CodeMatch(OpCodes.Br)
				)
				.ThrowIfInvalid("Could not find br instruction");
			codeMatcher.Advance(1);

			var fieldName = AccessTools.GetDeclaredFields(buildStartMoveNext.DeclaringType);
			var targetField = fieldName.Find((fieldInfo) => fieldInfo.Name.Contains("<tmpBuild>"));

			// We are now inside the while loop, before the first if check
			// Add call to TranspileBuildStart.RotateCheck()
			var newInstruction = new CodeInstruction(OpCodes.Ldarg_0);
			codeMatcher
				.InsertAndAdvance(
					newInstruction,
					new CodeInstruction(OpCodes.Ldfld, targetField),
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspileBuildStart), nameof(TranspileBuildStart.RotateCheck)))
				);
			codeMatcher.Instruction.MoveLabelsTo(newInstruction);

			return codeMatcher.Instructions();
		}
	}
}

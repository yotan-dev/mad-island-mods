using System.Collections;
using System.Collections.Generic;
using ExtendedHSystem.Handlers.Animation;
using ExtendedHSystem.Hook;
using ExtendedHSystem.Scenes;
using UnityEngine;
using YotanModCore;

namespace ExtendedHSystem.Mods
{
	/// <summary>
	/// Characters refuses insertion if sex meter is not at least at some point.
	/// Requiring "Foreplay" (Caressing) before being able to insert.
	/// </summary>
	public class RequireForeplay
	{
		private Dictionary<int, float> RandomModifiers = new Dictionary<int, float>();

		public void InitHooks()
		{
			HookBuilder.New("EHSMods.RequireForeplay.Main")
				.ForScenes(CommonSexPlayer.Name)
				.HookStepStart(CommonSexPlayer.StepNames.Main)
				.Call(this.OnStart);

			HookBuilder.New("EHSMods.RequireForeplay.Insert")
				.ForScenes(CommonSexPlayer.Name)
				.HookStepStart(CommonSexPlayer.StepNames.Insert)
				.Call(this.OnInsert);
		}

		private IEnumerator OnStart(IScene2 scene, object param)
		{
			var commonSexPlayer = scene as CommonSexPlayer;
			if (commonSexPlayer == null)
				yield break;

			var config = ExtendedHSystem.Config.Instance.RequireForeplay;
			var requiredBarValue = config.BarLevel.Value * 100f;
			
			if (config.UseAgeModifier.Value)
				requiredBarValue -= 3 * commonSexPlayer.Npc.age / 8f;

			if (config.UseRandomModifier.Value)
			{
				if (!RandomModifiers.ContainsKey(commonSexPlayer.Npc.friendID))
					RandomModifiers.Add(commonSexPlayer.Npc.friendID, Random.Range(0f, 10f));

				requiredBarValue -= RandomModifiers[commonSexPlayer.Npc.friendID];
			}

			requiredBarValue = Mathf.Clamp(requiredBarValue, 0f, 100f) / 100f;

			SexMeter.Instance.SetDividerPercent(requiredBarValue);
		}

		private IEnumerator OnInsert(IScene2 scene, object param)
		{
			var commonSexPlayer = scene as CommonSexPlayer;
			if (commonSexPlayer == null)
				yield break;

			if (SexMeter.Instance.FillAmount < SexMeter.Instance.DividerPercent)
			{
				string animName = commonSexPlayer.SexType + "Loop_01";
				float partialDuration = commonSexPlayer.CommonAnim.skeleton.Data.FindAnimation(animName).Duration / 2;
				
				yield return new LoopAnimationForTime(scene, commonSexPlayer.CommonAnim, animName, partialDuration).Handle();
				
				Managers.mn.uiMN.StartCoroutine(Managers.mn.eventMN.GoCautionSt(commonSexPlayer.Npc.charaName + " refuses to continue."));
				commonSexPlayer.Destroy();
			}
		}
	}
}

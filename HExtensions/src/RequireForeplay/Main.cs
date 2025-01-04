using System.Collections;
using System.Collections.Generic;
using HFramework.Handlers.Animation;
using HFramework.Hook;
using HFramework.Performer;
using HFramework.Scenes;
using UnityEngine;
using YotanModCore;

namespace HExtensions.RequireForeplay
{
	/// <summary>
	/// Characters refuses insertion if sex meter is not at least at some point.
	/// Requiring "Foreplay" (Caressing) before being able to insert.
	/// </summary>
	public class Main
	{
		private Dictionary<int, float> RandomModifiers = new Dictionary<int, float>();

		public void Init()
		{
			this.InitHooks();
		}

		public void InitHooks()
		{
			HookBuilder.New("HFMods.RequireForeplay.Main")
				.ForScenes(CommonSexPlayer.Name)
				.HookStepStart(CommonSexPlayer.StepNames.Main)
				.Call(this.OnStart);

			HookBuilder.New("HFMods.RequireForeplay.Insert")
				.ForScenes(CommonSexPlayer.Name)
				.HookStepStart(CommonSexPlayer.StepNames.Insert)
				.Call(this.OnInsert);
		}

		private IEnumerator OnStart(IScene scene, object param)
		{
			var commonSexPlayer = scene as CommonSexPlayer;
			if (commonSexPlayer == null)
				yield break;

			var config = RequireForeplayConfig.Instance;
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

		private IEnumerator OnInsert(IScene scene, object param)
		{
			var commonSexPlayer = scene as CommonSexPlayer;
			if (commonSexPlayer == null)
				yield break;

			if (SexMeter.Instance.FillAmount < SexMeter.Instance.DividerPercent)
			{
				var actionValue = scene.GetPerformer()?.GetActionValue(ActionType.Insert, out _);
				if (actionValue != null)
				{
					string animName = actionValue.AnimationName;
					float partialDuration = scene.GetSkelAnimation().skeleton.Data.FindAnimation(animName).Duration / 2;
					
					yield return new LoopAnimationForTime(scene, commonSexPlayer.CommonAnim, animName, partialDuration).Handle();
				}
				
				Managers.mn.uiMN.StartCoroutine(Managers.mn.eventMN.GoCautionSt(commonSexPlayer.Npc.charaName + " refuses to continue."));
				commonSexPlayer.Destroy();
			}
		}
	}
}

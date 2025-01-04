using System.Collections;
using HFramework.Hook;
using HFramework.ParamContainers;
using HFramework.Scenes;
using Spine.Unity;
using UnityEngine;
using YotanModCore.Consts;

namespace HExtensions.DickPainter
{
	/// <summary>
	/// Paints male dick in red if femaly is virgin.
	/// Paints male dick in pink if any actor is using a condom
	/// 
	/// @TODO: Improve the coloring sheme to consider some sort of blending with skin/condom/blood. Maybe also use the "water" color.
	/// </summary>
	public class Main
	{
		public void Init()
		{
			this.InitHooks();
		}

		public void InitHooks()
		{
			HookBuilder.New("HFMods.DickPainter.Main")
				.ForScenes(CommonSexPlayer.Name)
				.HookStepStart(CommonSexPlayer.StepNames.Main)
				.Call(this.OnStart);

			HookBuilder.New("HFMods.DickPainter.Penetrate")
				.ForScenes(CommonSexPlayer.Name)
				.HookEvent(EventNames.OnPenetrate)
				.CallBefore("HF.Friendly.OnPenetrate", this.OnPenetrate);
		}

		private IEnumerator OnStart(IScene scene, object param)
		{
			var actors = scene.GetActors();
			bool hasComdom = false;

			foreach (var actor in actors)
			{
				if (actor.equip[7].itemKey == "acce_s_01")
				{
					hasComdom = true;
					break;
				}
			}

			if (hasComdom)
			{
				// Condom pink
				scene.GetSkelAnimation()?.skeleton?.FindSlot("Tinko")?.SetColor(new Color32(250, 130, 197, 255));
			}

			yield break;
		}

		private IEnumerator OnPenetrate(IScene scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			if (fromTo.Value.To?.sexInfo[SexInfoIndex.FirstSex] == -1)
				scene.GetSkelAnimation()?.skeleton?.FindSlot("Tinko")?.SetColor(new Color32(255, 0, 0, 255));

			yield break;
		}
	}
}

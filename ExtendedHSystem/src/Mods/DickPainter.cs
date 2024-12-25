using System.Collections;
using ExtendedHSystem.Hook;
using ExtendedHSystem.ParamContainers;
using ExtendedHSystem.Scenes;
using Spine.Unity;
using UnityEngine;
using YotanModCore.Consts;

namespace ExtendedHSystem.Mods
{
	/// <summary>
	/// Paints male dick in red if femaly is virgin.
	/// Paints male dick in pink if any actor is using a condom
	/// 
	/// @TODO: Improve the coloring sheme to consider some sort of blending with skin/condom/blood. Maybe also use the "water" color.
	/// </summary>
	public class DickPainter
	{
		public void InitHooks()
		{
			HookBuilder.New("EHSMods.DickPainter.Main")
				.ForScenes(CommonSexPlayer.Name)
				.HookStepStart(CommonSexPlayer.StepNames.Main)
				.Call(this.OnStart);

			HookBuilder.New("EHSMods.DickPainter.Penetrate")
				.ForScenes(CommonSexPlayer.Name)
				.HookEvent(EventNames.OnPenetrate)
				.CallBefore("EHS.Friendly.OnPenetrate", this.OnPenetrate);
		}

		private IEnumerator OnStart(IScene2 scene, object param)
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

		private IEnumerator OnPenetrate(IScene2 scene, object param)
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

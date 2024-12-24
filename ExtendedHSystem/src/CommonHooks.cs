using System.Collections;
using ExtendedHSystem.Hook;
using ExtendedHSystem.ParamContainers;
using ExtendedHSystem.Scenes;
using YotanModCore;

namespace ExtendedHSystem
{
	public class CommonHooks
	{
		public static readonly CommonHooks Instance = new CommonHooks();

		private CommonHooks() { }

		public void InitHooks()
		{
			HookBuilder.New("EHS.Friendly.OnPenetrate")
				.ForScenes(CommonSexPlayer.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.OnPenetrate);
			HookBuilder.New("EHS.Any.OnCreampie")
				.ForScenes("*")
				.HookEvent(EventNames.OnCreampie)
				.Call(this.OnCreampie);

			HookBuilder.New("EHS.CommonSexPlayer.Affection")
				.ForScenes(CommonSexPlayer.Name)
				.HookStepEnd(CommonSexPlayer.StepNames.Main)
				.Call(this.OnCommonSexPlayerAffection);
		}

		private IEnumerator OnPenetrate(IScene2 scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			Managers.mn.sexMN.SexCountChange(fromTo.Value.From, fromTo.Value.To, SexManager.SexCountState.Normal);
			yield break;
		}

		private IEnumerator OnCreampie(IScene2 scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			Managers.mn.sexMN.SexCountChange(fromTo.Value.To, fromTo.Value.From, SexManager.SexCountState.Creampie);
			yield break;
		}

		private IEnumerator OnCommonSexPlayerAffection(IScene2 scene, object param)
		{
			var commonSexPlayer = scene as CommonSexPlayer;
			if (commonSexPlayer == null)
				yield break;

			if (commonSexPlayer.GetSexMeterFillAmount() == 1f)
				commonSexPlayer.Npc.LoveChange(commonSexPlayer.Player, 10f, false);
			else if (commonSexPlayer.GetSexMeterFillAmount() < 0.3f)
				commonSexPlayer.Npc.LoveChange(commonSexPlayer.Player, -5f, false);

			yield break;
		}
	}
}
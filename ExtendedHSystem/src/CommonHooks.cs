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

			HookBuilder.New("EHS.CommonSexNPC.Affection")
				.ForScenes(CommonSexNPC.Name)
				.HookStepEnd(CommonSexPlayer.StepNames.Main)
				.Call(this.OnCommonSexNPCAffection);
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

			if (SexMeter.Instance.FillAmount == 1f)
				commonSexPlayer.Npc.LoveChange(commonSexPlayer.Player, 10f, false);
			else if (SexMeter.Instance.FillAmount < 0.3f)
				commonSexPlayer.Npc.LoveChange(commonSexPlayer.Player, -5f, false);

			yield break;
		}

		private IEnumerator OnCommonSexNPCAffection(IScene2 scene, object param)
		{
			var commonSexNpc = scene as CommonSexNPC;
			if (commonSexNpc == null)
				yield break;

			commonSexNpc.NpcB.LoveChange(commonSexNpc.NpcA, 10f, false);
			commonSexNpc.NpcA.LoveChange(commonSexNpc.NpcB, 10f, false);

			yield break;
		}
	}
}
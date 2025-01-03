using System.Collections;
using System.Collections.Generic;
using HFramework.Hook;
using HFramework.Scenes;
using Gallery.GalleryScenes;
using Gallery.GalleryScenes.CommonSexPlayer;
using Gallery.GalleryScenes.CommonSexNPC;
using YotanModCore;
using Gallery.GalleryScenes.ManSleepRape;
using Gallery.GalleryScenes.ManRapes;
using Gallery.GalleryScenes.PlayerRaped;
using Gallery.GalleryScenes.AssWall;
using Gallery.GalleryScenes.Toilet;

namespace Gallery
{
	public class GalleryHooks
	{
		public static readonly GalleryHooks Instance = new GalleryHooks();

		private Dictionary<IScene2, BaseTracker> Trackers;

		private GalleryHooks()
		{
		}

		public void InitHooks()
		{
			Trackers = new Dictionary<IScene2, BaseTracker>();
			HookBuilder.New("Gallery.AssWall.Start")
				.ForScenes(AssWall.Name)
				.HookStepStart(AssWall.StepNames.Main)
				.Call(this.OnAssWallStart);
			HookBuilder.New("Gallery.CommonSexPlayer.Start")
				.ForScenes(CommonSexPlayer.Name)
				.HookStepStart(CommonSexPlayer.StepNames.Main)
				.Call(this.OnCommonSexPlayerStart);
			HookBuilder.New("Gallery.CommonSexNPC.Start")
				.ForScenes(CommonSexNPC.Name)
				.HookStepStart(CommonSexNPC.StepNames.Main)
				.Call(this.OnCommonSexNPCStart);
			HookBuilder.New("Gallery.ManRapes.Start")
				.ForScenes(ManRapes.Name)
				.HookStepStart(ManRapes.StepNames.Main)
				.Call(this.OnManRapesStart);
			HookBuilder.New("Gallery.ManRapesSleep.Start")
				.ForScenes(ManRapesSleep.Name)
				.HookStepStart(ManRapesSleep.StepNames.Main)
				.Call(this.OnManRapesSleepStart);
			HookBuilder.New("Gallery.PlayerRaped.Start")
				.ForScenes(PlayerRaped.Name)
				.HookStepStart(PlayerRaped.StepNames.Main)
				.Call(this.OnPlayerRapedStart);
			HookBuilder.New("Gallery.Toilet.Start")
				.ForScenes(Toilet.Name)
				.HookStepStart(Toilet.StepNames.Main)
				.Call(this.OnToiletStart);

			HookBuilder.New("Gallery.Scene.End")
				.ForScenes("*")
				.HookStepEnd(CommonSexNPC.StepNames.Main)
				.Call(this.OnSceneEnd);

			HookBuilder.New("Gallery.Friendly.OnPenetrate")
				.ForScenes(CommonSexPlayer.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.SetNormal);
			HookBuilder.New("Gallery.Rape.OnPenetrate")
				.ForScenes(ManRapes.Name, ManRapesSleep.Name, PlayerRaped.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.SetRaped);
			HookBuilder.New("Gallery.Toilet.OnPenetrate")
				.ForScenes(AssWall.Name, Toilet.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.SetToilet);

			HookBuilder.New("Gallery.Any.OnOrgasm")
				.ForScenes("*")
				.HookEvent(EventNames.OnOrgasm)
				.Call(this.SetBusted);
			HookBuilder.New("Gallery.Any.OnCreampie")
				.ForScenes("*")
				.HookEvent(EventNames.OnCreampie)
				.Call(this.SetCreampie);

			HookBuilder.New("Gallery.ManRapesSleep.ForceRape")
				.ForScenes(ManRapesSleep.Name)
				.HookStepStart(ManRapesSleep.StepNames.ForceRape)
				.Call((IScene2 scene, object param) => this.OnSetRapeMode(scene, ManRapeSleepState.ForcefullRape));
			HookBuilder.New("Gallery.ManRapesSleep.GentlyRape")
				.ForScenes(ManRapesSleep.Name)
				.HookStepStart(ManRapesSleep.StepNames.GentlyRape)
				.Call((IScene2 scene, object param) => this.OnSetRapeMode(scene, ManRapeSleepState.GentlyRape));
			HookBuilder.New("Gallery.ManRapesSleep.PowderRape")
				.ForScenes(ManRapesSleep.Name)
				.HookStepStart(ManRapesSleep.StepNames.PowderRape)
				.Call((IScene2 scene, object param) => this.OnSetRapeMode(scene, ManRapeSleepState.SleepPowder));
		}

		private IEnumerator OnSetRapeMode(IScene2 scene, ManRapeSleepState rapeType)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null && tracker is ManSleepRapeTracker manSleepRapeTracker)
				manSleepRapeTracker.OnSleepRapeTypeChange(rapeType);

			yield break;
		}

		private IEnumerator OnAssWallStart(IScene2 scene, object arg2)
		{
			var asswall = scene as AssWall;
			Trackers.Add(asswall, new AssWallTracker(asswall.Player, asswall.Npc, asswall.TmpWall.type));
			yield break;
		}

		private IEnumerator OnCommonSexPlayerStart(IScene2 scene, object arg2)
		{
			var commonSexPlayer = scene as CommonSexPlayer;
			Trackers.Add(commonSexPlayer, new CommonSexPlayerTracker(commonSexPlayer.Player, commonSexPlayer.Npc, commonSexPlayer.Type));
			yield break;
		}

		private IEnumerator OnCommonSexNPCStart(IScene2 scene, object arg2)
		{
			var commonSexNpc = scene as CommonSexNPC;

			// We only track if at least one is friend, as we can get some weird results otherwise -- specially with herb village
			if (CommonUtils.IsFriend(commonSexNpc.NpcA) || CommonUtils.IsFriend(commonSexNpc.NpcB))
				Trackers.Add(commonSexNpc, new CommonSexNPCTracker(commonSexNpc.NpcA, commonSexNpc.NpcB, commonSexNpc.Place, commonSexNpc.Type));

			yield break;
		}

		private IEnumerator OnManRapesStart(IScene2 scene, object arg2)
		{
			var manRapes = scene as ManRapes;
			Trackers.Add(manRapes, new ManRapesTracker(manRapes.Man, manRapes.Girl));
			yield break;
		}

		private IEnumerator OnManRapesSleepStart(IScene2 scene, object arg2)
		{
			var manRapes = scene as ManRapesSleep;
			Trackers.Add(manRapes, new ManSleepRapeTracker(manRapes.Man, manRapes.Girl));
			yield break;
		}

		private IEnumerator OnPlayerRapedStart(IScene2 scene, object arg2)
		{
			var manRapes = scene as PlayerRaped;
			Trackers.Add(manRapes, new PlayerRapedTracker(manRapes.Player, manRapes.Rapist));
			yield break;
		}

		private IEnumerator OnToiletStart(IScene2 scene, object arg2)
		{
			var toilet = scene as Toilet;
			Trackers.Add(toilet, new ToiletTracker(toilet.Player, toilet.Npc));
			yield break;
		}

		private IEnumerator OnSceneEnd(IScene2 scene, object arg2)
		{
			Trackers.GetValueOrDefault(scene, null)?.End();
			Trackers.Remove(scene);
			yield break;
		}

		private IEnumerator SetNormal(IScene2 scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.DidNormal = true;

			yield break;
		}

		private IEnumerator SetToilet(IScene2 scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.DidToilet = true;

			yield break;
		}

		private IEnumerator SetRaped(IScene2 scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.Raped = true;

			yield break;
		}

		private IEnumerator SetBusted(IScene2 scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.Busted = true;

			// Too lazy to make another hook
			if (scene is CommonSexPlayer commonSexPlayer)
			{
				if (tracker is CommonSexPlayerTracker commonSexPlayerTracker)
					commonSexPlayerTracker.SpecialFlag = commonSexPlayer.TmpSexCountType;
			}

			yield break;
		}

		private IEnumerator SetCreampie(IScene2 scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.DidCreampie = true;

			yield break;
		}
	}
}

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
using Gallery.GalleryScenes.Onani;
using Gallery.GalleryScenes.Delivery;
using Gallery.GalleryScenes.Slave;
using Gallery.GalleryScenes.Daruma;

namespace Gallery
{
	public class GalleryHooks
	{
		public static readonly GalleryHooks Instance = new GalleryHooks();

		private Dictionary<IScene, BaseTracker> Trackers;

		private GalleryHooks()
		{
		}

		public void InitHooks()
		{
			Trackers = new Dictionary<IScene, BaseTracker>();
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
			HookBuilder.New("Gallery.Daruma.Start")
				.ForScenes(Daruma.Name)
				.HookStepStart(Daruma.StepNames.Main)
				.Call(this.OnDarumaStart);
			HookBuilder.New("Gallery.Delivery.Start")
				.ForScenes(Delivery.Name)
				.HookStepStart(Delivery.StepNames.Main)
				.Call(this.OnDeliveryStart);
			HookBuilder.New("Gallery.ManRapes.Start")
				.ForScenes(ManRapes.Name)
				.HookStepStart(ManRapes.StepNames.Main)
				.Call(this.OnManRapesStart);
			HookBuilder.New("Gallery.ManRapesSleep.Start")
				.ForScenes(ManRapesSleep.Name)
				.HookStepStart(ManRapesSleep.StepNames.Main)
				.Call(this.OnManRapesSleepStart);
			HookBuilder.New("Gallery.Onani.Start")
				.ForScenes(OnaniNPC.Name)
				.HookStepStart(OnaniNPC.StepNames.Main)
				.Call(this.OnOnaniNPCStart);
			HookBuilder.New("Gallery.PlayerRaped.Start")
				.ForScenes(PlayerRaped.Name)
				.HookStepStart(PlayerRaped.StepNames.Main)
				.Call(this.OnPlayerRapedStart);
			HookBuilder.New("Gallery.Slave.Start")
				.ForScenes(Slave.Name)
				.HookStepStart(Slave.StepNames.Main)
				.Call(this.OnSlaveStart);
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
				.HookEvent(EventNames.OnPenetrateVag)
				.Call(this.SetNormal);
			HookBuilder.New("Gallery.Friendly.OnScissor")
				.ForScenes(CommonSexNPC.Name)
				.HookEvent(EventNames.OnScissor)
				.Call(this.SetNormal);
			HookBuilder.New("Gallery.Rape.OnPenetrate")
				.ForScenes(Daruma.Name, ManRapes.Name, ManRapesSleep.Name, PlayerRaped.Name)
				.HookEvent(EventNames.OnPenetrateVag)
				.Call(this.SetRaped);
			HookBuilder.New("Gallery.Toilet.OnPenetrate")
				.ForScenes(AssWall.Name, Toilet.Name)
				.HookEvent(EventNames.OnPenetrateVag)
				.Call(this.SetToilet);

			HookBuilder.New("Gallery.Any.OnMasturbate")
				.ForScenes("*")
				.HookEvent(EventNames.OnMasturbate)
				.Call(this.SetMasturbate);
			HookBuilder.New("Gallery.Any.OnOrgasm")
				.ForScenes("*")
				.HookEvent(EventNames.OnOrgasm)
				.Call(this.SetBusted);
			HookBuilder.New("Gallery.Any.OnCreampie")
				.ForScenes("*")
				.HookEvent(EventNames.OnCreampieVag)
				.Call(this.SetCreampie);

			HookBuilder.New("Gallery.ManRapesSleep.ForceRape")
				.ForScenes(ManRapesSleep.Name)
				.HookStepStart(ManRapesSleep.StepNames.ForceRape)
				.Call((IScene scene, object param) => this.OnSetRapeMode(scene, ManRapeSleepState.ForcefullRape));
			HookBuilder.New("Gallery.ManRapesSleep.GentlyRape")
				.ForScenes(ManRapesSleep.Name)
				.HookStepStart(ManRapesSleep.StepNames.GentlyRape)
				.Call((IScene scene, object param) => this.OnSetRapeMode(scene, ManRapeSleepState.GentlyRape));
			HookBuilder.New("Gallery.ManRapesSleep.PowderRape")
				.ForScenes(ManRapesSleep.Name)
				.HookStepStart(ManRapesSleep.StepNames.PowderRape)
				.Call((IScene scene, object param) => this.OnSetRapeMode(scene, ManRapeSleepState.SleepPowder));
		}

		private IEnumerator OnSetRapeMode(IScene scene, ManRapeSleepState rapeType)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null && tracker is ManSleepRapeTracker manSleepRapeTracker)
				manSleepRapeTracker.OnSleepRapeTypeChange(rapeType);

			yield break;
		}

		private IEnumerator OnAssWallStart(IScene scene, object arg2)
		{
			var asswall = scene as AssWall;
			Trackers.Add(asswall, new AssWallTracker(asswall.Player, asswall.Npc, asswall.TmpWall.type));
			yield break;
		}

		private IEnumerator OnCommonSexPlayerStart(IScene scene, object arg2)
		{
			var commonSexPlayer = scene as CommonSexPlayer;
			Trackers.Add(commonSexPlayer, new CommonSexPlayerTracker(commonSexPlayer.Player, commonSexPlayer.Npc, commonSexPlayer.Type));
			yield break;
		}

		private IEnumerator OnCommonSexNPCStart(IScene scene, object arg2)
		{
			var commonSexNpc = scene as CommonSexNPC;

			// We only track if at least one is friend, as we can get some weird results otherwise -- specially with herb village
			if (CommonUtils.IsFriend(commonSexNpc.Npc1) || CommonUtils.IsFriend(commonSexNpc.Npc2))
				Trackers.Add(commonSexNpc, new CommonSexNPCTracker(commonSexNpc.Npc1, commonSexNpc.Npc2, commonSexNpc.Place, commonSexNpc.Type));

			yield break;
		}

		private IEnumerator OnDarumaStart(IScene scene, object arg2)
		{
			var daruma = scene as Daruma;
			Trackers.Add(daruma, new DarumaTracker(daruma.Player, daruma.Npc));

			yield break;
		}

		private IEnumerator OnDeliveryStart(IScene scene, object arg2)
		{
			var delivery = scene as Delivery;
			Trackers.Add(delivery, new DeliveryTracker(delivery.Girl));
			yield break;
		}

		private IEnumerator OnManRapesStart(IScene scene, object arg2)
		{
			var manRapes = scene as ManRapes;
			Trackers.Add(manRapes, new ManRapesTracker(manRapes.Man, manRapes.Girl));
			yield break;
		}

		private IEnumerator OnManRapesSleepStart(IScene scene, object arg2)
		{
			var manRapes = scene as ManRapesSleep;
			Trackers.Add(manRapes, new ManSleepRapeTracker(manRapes.Man, manRapes.Girl));
			yield break;
		}

		private IEnumerator OnOnaniNPCStart(IScene scene, object arg2)
		{
			var onani = scene as OnaniNPC;
			Trackers.Add(onani, new OnaniTracker(onani.Npc));
			yield break;
		}

		private IEnumerator OnPlayerRapedStart(IScene scene, object arg2)
		{
			var manRapes = scene as PlayerRaped;
			Trackers.Add(manRapes, new PlayerRapedTracker(manRapes.Player, manRapes.Rapist));
			yield break;
		}

		private IEnumerator OnSlaveStart(IScene scene, object arg2)
		{
			var slave = scene as Slave;
			Trackers.Add(slave, new SlaveTracker(slave.Player, slave.TmpSlave));
			yield break;
		}

		private IEnumerator OnToiletStart(IScene scene, object arg2)
		{
			var toilet = scene as Toilet;
			Trackers.Add(toilet, new ToiletTracker(toilet.Player, toilet.Npc));
			yield break;
		}

		private IEnumerator OnSceneEnd(IScene scene, object arg2)
		{
			Trackers.GetValueOrDefault(scene, null)?.End();
			Trackers.Remove(scene);
			yield break;
		}

		private IEnumerator SetNormal(IScene scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.DidNormal = true;

			yield break;
		}

		private IEnumerator SetToilet(IScene scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.DidToilet = true;

			yield break;
		}

		private IEnumerator SetRaped(IScene scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.Raped = true;

			yield break;
		}

		private IEnumerator SetMasturbate(IScene scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.DidMasturbation = true;

			yield break;
		}

		private IEnumerator SetBusted(IScene scene, object param)
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

		private IEnumerator SetCreampie(IScene scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.DidCreampie = true;

			yield break;
		}
	}
}

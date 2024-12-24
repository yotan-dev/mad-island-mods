using System;
using System.Collections;
using System.Collections.Generic;
using ExtendedHSystem.Hook;
using ExtendedHSystem.ParamContainers;
using ExtendedHSystem.Scenes;
using Gallery.GalleryScenes;
using Gallery.GalleryScenes.CommonSexPlayer;

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
			HookBuilder.New("Gallery.CommonSexPlayer.Start")
				.ForScenes(CommonSexPlayer.Name)
				.HookStepStart(CommonSexPlayer.StepNames.Main)
				.Call(this.OnCommonSexPlayerStart);

			HookBuilder.New("Gallery.Scene.End")
				.ForScenes("*")
				.HookStepStart("Main")
				.Call(this.OnSceneEnd);

			HookBuilder.New("Gallery.Friendly.OnPenetrate")
				.ForScenes(CommonSexPlayer.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.SetNormal);
			HookBuilder.New("Gallery.Any.OnOrgasm")
				.ForScenes("*")
				.HookEvent(EventNames.OnOrgasm)
				.Call(this.SetBusted);
			HookBuilder.New("Gallery.Any.OnCreampie")
				.ForScenes("*")
				.HookEvent(EventNames.OnCreampie)
				.Call(this.SetCreampie);
		}

		private IEnumerator OnCommonSexPlayerStart(IScene2 scene, object arg2)
		{
			var commonSexPlayer = scene as CommonSexPlayer;
			Trackers.Add(commonSexPlayer, new CommonSexPlayerTracker(commonSexPlayer.Player, commonSexPlayer.Npc, commonSexPlayer.Type));
			yield break;
		}

		private IEnumerator OnSceneEnd(IScene2 scene, object arg2)
		{
			Trackers.GetValueOrDefault(scene, null)?.End();
			yield break;
		}

		private IEnumerator SetNormal(IScene2 scene, object param)
		{
			var tracker = Trackers.GetValueOrDefault(scene, null);
			if (tracker != null)
				tracker.DidNormal = true;
			
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
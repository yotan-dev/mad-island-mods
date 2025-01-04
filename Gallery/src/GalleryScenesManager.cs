using System;
using System.Collections.Generic;
using HFramework;
using Gallery.GalleryScenes;
using Gallery.GalleryScenes.AssWall;
using Gallery.GalleryScenes.CommonSexNPC;
using Gallery.GalleryScenes.CommonSexPlayer;
using Gallery.GalleryScenes.Daruma;
using Gallery.GalleryScenes.Delivery;
using Gallery.GalleryScenes.ManRapes;
using Gallery.GalleryScenes.ManSleepRape;
using Gallery.GalleryScenes.Onani;
using Gallery.GalleryScenes.PlayerRaped;
using Gallery.GalleryScenes.Slave;
using Gallery.GalleryScenes.Toilet;
using Gallery.GalleryScenes.ToiletNpc;
using YotanModCore;

namespace Gallery
{
	public class GalleryScenesManager
	{
		public static GalleryScenesManager Instance { get; set; }

		private Dictionary<CommonStates, BaseTracker> Trackers = new Dictionary<CommonStates, BaseTracker>();

		public List<ISceneManager> SceneManagers = new List<ISceneManager>()
		{
			ManRapesSceneManager.Instance,
			DarumaSceneManager.Instance,
			SlaveSceneManager.Instance,
			CommonSexNPCSceneManager.Instance,
			CommonSexPlayerSceneManager.Instance,
			AssWallSceneManager.Instance,
			DeliverySceneManager.Instance,
			ManSleepRapeSceneManager.Instance,
			PlayerRapedSceneManager.Instance,
			new ToiletNpcSceneManager(),
			ToiletSceneManager.Instance,
		};

		public static void Init() {
			Instance = new GalleryScenesManager();
			if (GameInfo.GameVersion >= GameInfo.ToVersion("0.1.0")) {
				Instance.SceneManagers.Add(OnaniSceneManager.Instance);
			}
		}

		public BaseTracker GetTrackerForCommon(CommonStates common)
		{
			return this.Trackers.GetValueOrDefault(common, null);
		}

		public void AddTrackerForCommon(CommonStates common, BaseTracker tracker)
		{
			if (this.Trackers.ContainsKey(common))
				this.Trackers[common] = tracker;
			else
				this.Trackers.Add(common, tracker);
		}

		public void RemoveTrackerForCommon(CommonStates common)
		{
			if (this.Trackers.ContainsKey(common))
				this.Trackers.Remove(common);
		}
	}
}

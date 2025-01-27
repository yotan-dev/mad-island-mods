using System;
using System.Collections.Generic;
using Gallery.GalleryScenes;
using Gallery.GalleryScenes.Daruma;
using Gallery.GalleryScenes.ToiletNpc;
using System.Xml.Serialization;
using Gallery.ConfigFiles;
using System.IO;

namespace Gallery
{
	public class GalleryScenesManager
	{
		public static GalleryScenesManager Instance { get; set; }

		private Dictionary<CommonStates, BaseTracker> Trackers = new Dictionary<CommonStates, BaseTracker>();

		public Dictionary<string, GalleryGroupConfig> SceneGroups = [];

		public static void Init()
		{
			Instance = new GalleryScenesManager();
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

		public void LoadGallery()
		{
			if (this.SceneGroups.Count > 0)
				return;


			XmlSerializer serializer = new XmlSerializer(typeof(GalleryGroupsConfig));
			var fileStream = new FileStream("BepInEx/plugins/Gallery/GalleryList.xml", FileMode.Open);
			var scenesConfig = (GalleryGroupsConfig)serializer.Deserialize(fileStream);
			fileStream.Close();

			foreach (var group in scenesConfig.Groups)
			{
				this.SceneGroups.Add(group.Name, group);
			}
		}
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using Gallery.GalleryScenes;
using System.Xml.Serialization;
using Gallery.ConfigFiles;
using System.IO;
using Gallery.SaveFile.Containers;
using HFramework.Performer;

namespace Gallery
{
	public class GalleryScenesManager
	{
		public static GalleryScenesManager Instance { get; set; } = new GalleryScenesManager();

		private Dictionary<CommonStates, BaseTracker?> Trackers = [];

		public Dictionary<string, GalleryGroupConfig> SceneGroups = [];

		private Dictionary<string, List<string>> ControllerPerformers = [];

		public BaseTracker? GetTrackerForCommon(CommonStates common)
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

				foreach (var scene in group.Scenes)
				{
					var ctrlerType = scene.Controller.GetType().ToString();
					if (!this.ControllerPerformers.TryGetValue(ctrlerType, out var performers))
					{
						performers = [];
						this.ControllerPerformers.Add(ctrlerType, performers);
					}

					performers.Add(scene.Controller.PerformerId);
				}
			}
		}

		public bool HasPerformerForController(Type controllerType, string performerId)
		{
			if (!this.ControllerPerformers.TryGetValue(controllerType.ToString(), out var performers))
				return false;

			return performers.Contains(performerId);
		}

		public string FindPerformer(Type controllerType, GalleryChara[] charas)
		{
			if (!this.ControllerPerformers.TryGetValue(controllerType.ToString(), out var performers))
				return "";

			foreach (var performerId in performers)
			{
				PerformerLoader.Performers.TryGetValue(performerId, out var performer);

				if (performer == null)
					continue;

				int numCharas = 0;
				if (performer.FromNpcId != -1)
					numCharas++;

				if (performer.ToNpcId != null && performer.ToNpcId != -1)
					numCharas++;

				if (numCharas != charas.Length)
					continue;

				// First char is the same
				if (performer.FromNpcId != charas[0].Id)
					continue;

				// Second char is the same
				if (performer.ToNpcId != null && performer.ToNpcId != charas[1].Id)
					continue;

				// This performer is probably right; now we need to treat edge cases

				if (performerId == "HF_YoungMan_FemaleNative_Friendly_Normal" || performerId == "HF_YoungMan_FemaleNative_Friendly_Pregnant")
				{
					if (charas[1].IsPregnant)
						return "HF_YoungMan_FemaleNative_Friendly_Pregnant";
					else
						return "HF_YoungMan_FemaleNative_Friendly_Normal";
				}

				if (performerId == "HF_Man_FemaleNative_Friendly_Normal" || performerId == "HF_Man_FemaleNative_Friendly_Pregnant")
				{
					if (charas[1].IsPregnant)
						return "HF_Man_FemaleNative_Friendly_Pregnant";
					else
						return "HF_Man_FemaleNative_Friendly_Normal";
				}

				if (performerId == "HF_Man_FemaleNative_Rape_Fainted" || performerId == "HF_Man_FemaleNative_Rape_Grapple" || performerId == "HF_Man_FemaleNative_Rape_Grapple_Pregnant")
				{
					if (charas[1].IsPregnant)
						return "HF_Man_FemaleNative_Rape_Grapple_Pregnant";

					if (charas[1].IsFainted)
						return "HF_Man_FemaleNative_Rape_Fainted";

					return "HF_Man_FemaleNative_Rape_Grapple";
				}

				if (performerId == "HF_Man_NativeGirl_Rape_Fainted" || performerId == "HF_Man_NativeGirl_Rape_Grapple")
				{
					if (charas[1].IsFainted)
						return "HF_Man_NativeGirl_Rape_Fainted";

					return "HF_Man_NativeGirl_Rape_Grapple";
				}

				if (performerId == "HF_FemaleLargeNative_Man_Rape_Battle")
					return "HF_FemaleLargeNative_Man_Rape_Sex";

				// @TODO: Can't differentiate:
				// HF_Man_FemaleLargeNative_Friendly_Cowgirl_Normal / HF_Man_FemaleLargeNative_Friendly_Doggy_Normal
				// HF_Man_Reika_Friendly_Cowgirl / HF_Man_Reika_Friendly_RevCowgirl
				// HF_Man_Mermaid_Friendly_TittyFuck / HF_Man_Mermaid_Friendly_Fuck
				// HF_Man_Shino_Friendly_TittyFuck / HF_Man_Shino_Friendly_Fuck

				return performerId;
			}

			return "";
		}
	}
}

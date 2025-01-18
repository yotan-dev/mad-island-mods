#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using HFramework.ConfigFiles;

namespace HFramework.Scenes
{
	public static class ScenesLoader
	{
		internal static void RegisterScenes()
		{
			ScenesManager.Instance.AddScene(AssWall.Name, new SceneInfo(AssWall.Name));
			ScenesManager.Instance.AddScene(CommonSexNPC.Name, new SceneInfo(CommonSexNPC.Name));
			ScenesManager.Instance.AddScene(CommonSexPlayer.Name, new SceneInfo(CommonSexPlayer.Name));
			ScenesManager.Instance.AddScene(Daruma.Name, new SceneInfo(Daruma.Name));
			ScenesManager.Instance.AddScene(Delivery.Name, new SceneInfo(Delivery.Name));
			ScenesManager.Instance.AddScene(ManRapes.Name, new SceneInfo(ManRapes.Name));
			ScenesManager.Instance.AddScene(ManRapesSleep.Name, new SceneInfo(ManRapesSleep.Name));
			ScenesManager.Instance.AddScene(OnaniNPC.Name, new SceneInfo(OnaniNPC.Name));
			ScenesManager.Instance.AddScene(PlayerRaped.Name, new SceneInfo(PlayerRaped.Name));
			ScenesManager.Instance.AddScene(Slave.Name, new SceneInfo(Slave.Name));
			ScenesManager.Instance.AddScene(Toilet.Name, new SceneInfo(Toilet.Name));
		}

		internal static void RegisterScenePerformers()
		{
			string errorMessage = "";
			try
			{
				var serializer = new XmlSerializer(typeof(ScenesConfig));
				var fileStream = new FileStream("BepInEx/plugins/HFramework/Scenes.xml", FileMode.Open);
				var scenesConfig = (ScenesConfig)serializer.Deserialize(fileStream);
				fileStream.Close();

				foreach (var scene in scenesConfig.Scenes)
				{
					PLogger.LogDebug($"Loading info for scene {scene.Id}");
					var sceneInfo = ScenesManager.Instance.GetSceneInfo(scene.Id);
					if (sceneInfo == null)
					{
						PLogger.LogError($"Unknown scene: {scene.Id}");
						continue;
					}

					foreach (var performerConfig in scene.Performers)
					{
						var performerConst = performerConfig.Performer;
						var desc = $"(Scene: {scene.Id} / Performer: {performerConst})";
						PLogger.LogDebug($"Loading performer {desc}");

						var performer = Performer.PerformerLoader.Performers.GetValueOrDefault(performerConst);
						if (performer == null)
						{
							PLogger.LogError($"Unknown performer: {performerConst} {desc}");
							continue;
						}

						errorMessage = $"Failed to load Performer {performerConst} {desc}";
						sceneInfo.AddPerformer(performer, performerConfig.StartConditions, performerConfig.PerformConditions);
					}
				}
			}
			catch (Exception e)
			{
				PLogger.LogError($"Failed to load scenes: {errorMessage}. {e.Message}");
				PLogger.LogError(e.StackTrace);
			}
		}
	}
}

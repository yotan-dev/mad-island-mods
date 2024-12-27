using System.Collections.Generic;
using System.IO;
using ExtendedHSystem.ConfigFiles;
using Tomlyn;

namespace ExtendedHSystem.Scenes
{
	public static class ScenesLoader
	{
		public delegate void RegisterScenes();

		public static event RegisterScenes OnRegisterScenes;

		public delegate void RegisterScenePerformers();

		public static event RegisterScenePerformers OnRegisterScenePerformers;

		public static Dictionary<string, SceneInfo> SceneInfos = new Dictionary<string, SceneInfo>();

		private static void RegisterEHSScenes()
		{
			SceneInfos.Add(CommonSexNPC.Name, new SceneInfo(CommonSexNPC.Name));
			SceneInfos.Add(CommonSexPlayer.Name, new SceneInfo(CommonSexPlayer.Name));
			SceneInfos.Add(PlayerRaped.Name, new SceneInfo(PlayerRaped.Name));

			ScenesLoader.OnRegisterScenes?.Invoke();
		}

		public static void Load()
		{
			PLogger.LogInfo("Registering scenes...");
			ScenesLoader.RegisterEHSScenes();

			PLogger.LogInfo("Loading scenes...");
			
			var scenesConfigTxt = File.ReadAllText("BepInEx/plugins/ExtendedHSystem/Scenes.toml");
			var scenesConfig = Toml.ToModel<ScenesConfig>(scenesConfigTxt, "Scenes.toml", new TomlModelOptions() { ConvertPropertyName = (name) => name });

			foreach (var scene in scenesConfig.Scenes)
			{
				var sceneInfo = SceneInfos.GetValueOrDefault(scene.Id, null);
				if (sceneInfo == null)
				{
					PLogger.LogError($"Unknown scene: {scene.Id}");
					continue;
				}

				foreach (var performerConst in scene.Performers)
				{
					var performer = Performer.PerformerLoader.Performers.GetValueOrDefault(performerConst, null);
					if (performer == null)
					{
						PLogger.LogError($"Unknown performer: {performerConst} for scene: {scene.Id}");
						continue;
					}

					sceneInfo.AddPerformer(performer);
				}
			}

			ScenesLoader.OnRegisterScenePerformers?.Invoke();

			PLogger.LogInfo("Scenes loaded");
		}
	}
}

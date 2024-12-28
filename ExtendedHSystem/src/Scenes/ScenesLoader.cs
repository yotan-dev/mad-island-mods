using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExtendedHSystem.ConfigFiles;
using ExtendedHSystem.Scenes.Conditionals;
using Tomlyn;
using YotanModCore;

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

		private static int ParseActor(string actor)
		{
			var parts = actor.Split('#');

			var npcId = CommonUtils.ConstToId(parts[0]);
			if (parts.Length >= 2 && int.TryParse(parts[1], out var result))
			{
				if (npcId != result)
					PLogger.LogWarning($"Actor {actor} has unmatching constant vs ID. Constant: {parts[0]} ({npcId}) / ID: {result}");
			}

			return npcId;
		}

		private static IConditional ParseCondition(ConditionsConfig config)
		{
			if (config.Type == "PregnantCheck")
				return new PregnantCheck(ParseActor((string)config.Args[0]), (bool)config.Args[1]);

			if (config.Type == "SexTypeCheck")
				return new SexTypeCheck((int)(long)config.Args[0]);

			if (config.Type == "QuestProgressCheck")
			{
				if (config.Args[2].GetType() != typeof(long))
				{
					var vals = (Tomlyn.Model.TomlArray)config.Args[2];
					var intVals = vals.Select((v) => (int)(long)v).ToArray();
					return new QuestProgressCheck((string)config.Args[0], (string)config.Args[1], intVals);
				}

				return new QuestProgressCheck((string)config.Args[0], (string)config.Args[1], (int)(long)config.Args[2]);
			}

			PLogger.LogError($"Unknown condition type {config.Type}. Ignoring...");
			return null;
		}

		public static void Load()
		{
			string errorMessage = "";
			try
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

					foreach (var performerConfig in scene.Performers)
					{
						var performerConst = performerConfig.Performer;
						var desc = $"(Scene: {scene.Id} / Performer: {performerConst})";

						var performer = Performer.PerformerLoader.Performers.GetValueOrDefault(performerConst, null);
						if (performer == null)
						{
							PLogger.LogError($"Unknown performer: {performerConst} {desc}");
							continue;
						}

						List<IConditional> startConditions = new List<IConditional>();
						errorMessage = $"Failed to load StartConditions {desc}";
						foreach (var condition in performerConfig.StartConditions)
						{
							errorMessage = $"Failed to load StartCondition {condition.Type} {desc}";

							var cond = ParseCondition(condition);
							if (cond != null)
								startConditions.Add(cond);
						}

						List<IConditional> performConditions = new List<IConditional>();
						errorMessage = $"Failed to load PerformConditions {desc}";
						foreach (var condition in performerConfig.PerformConditions)
						{
							errorMessage = $"Failed to load PerformCondition {condition.Type} {desc}";

							var cond = ParseCondition(condition);
							if (cond != null)
								performConditions.Add(cond);
						}

						errorMessage = $"Failed to load Performer {performerConst} {desc}";
						sceneInfo.AddPerformer(performer, startConditions.ToArray(), performConditions.ToArray());
					}
				}

				ScenesLoader.OnRegisterScenePerformers?.Invoke();

				PLogger.LogInfo("Scenes loaded");
			}
			catch (Exception e)
			{
				PLogger.LogError($"Failed to load scenes: {errorMessage}. {e.Message}");
				PLogger.LogError(e.StackTrace);
			}
		}
	}
}

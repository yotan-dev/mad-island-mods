#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HFramework.ConfigFiles;
using HFramework.Scenes.Conditionals;
using Tomlyn;
using YotanModCore;

namespace HFramework.Scenes
{
	public static class ScenesLoader
	{
		internal static void RegisterScenes()
		{
			ScenesManager.Instance.AddScene(CommonSexNPC.Name, new SceneInfo(CommonSexNPC.Name));
			ScenesManager.Instance.AddScene(CommonSexPlayer.Name, new SceneInfo(CommonSexPlayer.Name));
			ScenesManager.Instance.AddScene(ManRapes.Name, new SceneInfo(ManRapes.Name));
			ScenesManager.Instance.AddScene(ManRapesSleep.Name, new SceneInfo(ManRapesSleep.Name));
			ScenesManager.Instance.AddScene(PlayerRaped.Name, new SceneInfo(PlayerRaped.Name));
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

		private static IConditional? ParseCondition(ConditionsConfig config)
		{
			if (config.Type == "PregnantCheck")
				return new PregnantCheck(ParseActor((string)config.Args[0]), (bool)config.Args[1]);

			if (config.Type == "SexTypeCheck")
				return new SexTypeCheck((int)(long)config.Args[0]);

			if (config.Type == "LibidoCheck")
				return new LibidoCheck(ParseActor((string) config.Args[0]), (string) config.Args[1], (float)(double)config.Args[2]);

			if (config.Type == "QuestProgressCheck")
			{
				if (config.Args[2].GetType() != typeof(long))
				{
					var vals = (Tomlyn.Model.TomlArray)config.Args[2];
					var intVals = vals.Select((v) => (int)(long)v!).ToArray();
					return new QuestProgressCheck((string)config.Args[0], (string)config.Args[1], intVals);
				}

				return new QuestProgressCheck((string)config.Args[0], (string)config.Args[1], (int)(long)config.Args[2]);
			}

			if (config.Type == "FaintCheck")
				return new FaintCheck(ParseActor((string)config.Args[0]), (bool)config.Args[1]);

			if (config.Type == "FriendCheck")
				return new FriendCheck(ParseActor((string)config.Args[0]), (bool)config.Args[1]);

			if (config.Type == "JokeCheck")
				return new JokeCheck((bool)config.Args[0]);

			PLogger.LogError($"Unknown condition type {config.Type}. Ignoring...");
			return null;
		}

		internal static void RegisterScenePerformers()
		{
			string errorMessage = "";
			try
			{
				var scenesConfigTxt = File.ReadAllText("BepInEx/plugins/HFramework/Scenes.toml");
				var scenesConfig = Toml.ToModel<ScenesConfig>(scenesConfigTxt, "Scenes.toml", new TomlModelOptions() { ConvertPropertyName = (name) => name });

				foreach (var scene in scenesConfig.Scenes)
				{
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

						var performer = Performer.PerformerLoader.Performers.GetValueOrDefault(performerConst);
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
			}
			catch (Exception e)
			{
				PLogger.LogError($"Failed to load scenes: {errorMessage}. {e.Message}");
				PLogger.LogError(e.StackTrace);
			}
		}
	}
}

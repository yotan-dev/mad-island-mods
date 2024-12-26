using System.Collections.Generic;
using System.IO;
using ExtendedHSystem.ConfigFiles;
using Tomlyn;
using YotanModCore;

namespace ExtendedHSystem.Performer
{
	public class PerformerLoader
	{
		public static Dictionary<string, SexPerformerInfo> Performers = new Dictionary<string, SexPerformerInfo>();
		private static Dictionary<string, ActionType> ConstToActionType = new Dictionary<string, ActionType>()
		{
			{ "StartIdle", ActionType.StartIdle },
			{ "Caress", ActionType.Caress },
			{ "Insert", ActionType.Insert },
			{ "Speed", ActionType.Speed },
			{ "Finish", ActionType.Finish },
			{ "FinishIdle", ActionType.FinishIdle },
		};

		private static Dictionary<string, PlayType> ConstToPlayType = new Dictionary<string, PlayType>()
		{
			{ "Loop", PlayType.Loop },
			{ "Once", PlayType.Once },
		};

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
				return new SexTypeCheck((int) (long) config.Args[0]);

			PLogger.LogError($"Unknown condition type {config.Type}. Ignoring...");
			return null;
		}

		public static void Load()
		{
			PLogger.LogInfo("Loading performers");

			var scenesConfigTxt = File.ReadAllText("BepInEx/plugins/ExtendedHSystem/Performers.toml");
			var scenesConfig = Toml.ToModel<PerformersConfig>(scenesConfigTxt, "Performers.toml", new TomlModelOptions() { ConvertPropertyName = (name) => name });

			foreach (var scene in scenesConfig.Performers)
			{
				string errorMessage = "";
				var builder = new SexPerformerInfoBuilder(scene.Id);

				try
				{
					errorMessage = $"Failed to load Prefab {scene.Prefab.Type}";
					if (scene.Prefab.Type == "SexList")
						builder.SetSexPrefabSelector(new SexListPrefabSelector((int) (long)scene.Prefab.Args[0], (int) (long) scene.Prefab.Args[1]));
					else
						PLogger.LogError($"Unknown prefab type {scene.Prefab.Type}");

					errorMessage = $"Failed to load Actors";

					int fromNpc = -1;
					int? toNpc = null;

					if (scene.Actors.Length >= 1)
						fromNpc = ParseActor(scene.Actors[0]);

					if (scene.Actors.Length >= 2)
						toNpc = ParseActor(scene.Actors[1]);

					if (fromNpc != -1)
						builder.SetActors(fromNpc, toNpc);
					else
						PLogger.LogError($"Unknown actora for scene {scene.Id}");

					foreach (var condition in scene.Conditions)
					{
						errorMessage = $"Failed to load Condition {condition.Type}";

						var cond = ParseCondition(condition);
						if (cond != null)
							builder.AddCondition(cond);
					}

					foreach (var animation in scene.Animations)
					{
						errorMessage = $"Failed to load Animation {animation.Action}";

						var action = ConstToActionType.GetValueOrDefault(animation.Action, ActionType.None);
						if (action == ActionType.None)
						{
							PLogger.LogError($"Unknown action type {animation.Action}");
							continue;
						}

						var playType = ConstToPlayType.GetValueOrDefault(animation.Play, PlayType.None);
						if (playType == PlayType.None)
						{
							PLogger.LogError($"Unknown play type {animation.Play}");
							continue;
						}

						var pose = animation.Pose ?? 1;

						builder.AddAnimation(action, pose, new ActionValue(playType, animation.Name));
					}

					Performers.Add(scene.Id, builder.Build());
				}
				catch (System.Exception ex)
				{
					PLogger.LogError($"Failed to load performer {scene.Id}: {errorMessage}");
					PLogger.LogError(ex.Message);
				}
			}

			PLogger.LogInfo("Performers loaded");
		}
	}
}

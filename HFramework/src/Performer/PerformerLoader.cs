#nullable enable

using System.Collections.Generic;
using System.IO;
using HFramework.ConfigFiles;
using Tomlyn;
using YotanModCore;

namespace HFramework.Performer
{
	public class PerformerLoader
	{
		public delegate void LoadPeformers();

		public static event LoadPeformers? OnLoadPeformers;

		public static Dictionary<string, SexPerformerInfo> Performers = new Dictionary<string, SexPerformerInfo>();
		private static Dictionary<string, ActionType> ConstToActionType = new Dictionary<string, ActionType>()
		{
			{ "Battle", ActionType.Battle },
			{ "Attack", ActionType.Attack },
			{ "Defeat", ActionType.Defeat },
			{ "StartIdle", ActionType.StartIdle },
			{ "IdlePee", ActionType.IdlePee },
			{ "Caress", ActionType.Caress },
			{ "Insert", ActionType.Insert },
			{ "InsertIdle", ActionType.InsertIdle },
			{ "InsertPee", ActionType.InsertPee },
			{ "Speed1", ActionType.Speed1 },
			{ "Speed2", ActionType.Speed2 },
			{ "Speed3", ActionType.Speed3 },
			{ "Finish", ActionType.Finish },
			{ "FinishIdle", ActionType.FinishIdle },
		};

		private static Dictionary<string, PlayType> ConstToPlayType = new Dictionary<string, PlayType>()
		{
			{ "Loop", PlayType.Loop },
			{ "Once", PlayType.Once },
		};

		private static Dictionary<string, PerformerScope> ConstToScope = new Dictionary<string, PerformerScope>()
		{
			{ "Battle", PerformerScope.Battle },
			{ "Sex", PerformerScope.Sex },
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

		public static void Load()
		{
			PLogger.LogInfo("Loading performers");

			var scenesConfigTxt = File.ReadAllText("BepInEx/plugins/HFramework/Performers.toml");
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
					else if (scene.Prefab.Type == "SexObj")
						builder.SetSexPrefabSelector(new SexObjPrefabSelector((int) (long)scene.Prefab.Args[0]));
					else
						PLogger.LogError($"Unknown prefab type {scene.Prefab.Type}");

					errorMessage = "Failed to load scopes";
					foreach (var scope in scene.Scopes)
					{
						errorMessage = $"Failed to load Scope {scope}";
						var scopeType = ConstToScope.GetValueOrDefault(scope, PerformerScope.None);
						if (scopeType != PerformerScope.None)
							builder.AddScope(scopeType);
						else
							PLogger.LogError($"Unknown scope type {scope}");
					}

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

					foreach (var animSet in scene.AnimationSets)
					{
						errorMessage = $"Failed to load Animation Set {animSet.Key}";

						var animSetBuilder = new AnimationSetBuilder(animSet.Key);
						foreach (var anim in animSet.Value)
						{
							errorMessage = $"Failed to load Animation {anim.Action}";

							var action = ConstToActionType.GetValueOrDefault(anim.Action, ActionType.None);
							if (action == ActionType.None)
							{
								PLogger.LogError($"Unknown action type {anim.Action} for set {animSet.Key}");
								continue;
							}

							var playType = ConstToPlayType.GetValueOrDefault(anim.Play, PlayType.None);
							if (playType == PlayType.None)
							{
								PLogger.LogError($"Unknown play type {anim.Play} for set {animSet.Key}");
								continue;
							}

							var pose = anim.Pose ?? 1;
							var canChangePose = anim.ChangePose ?? true;
							animSetBuilder.AddAnimation(action, pose, new ActionValue(playType, anim.Name, anim.Events, canChangePose));
						}
						builder.AddAnimationSet(animSetBuilder.Build());
					}

					Performers.Add(scene.Id, builder.Build());
				}
				catch (System.Exception ex)
				{
					PLogger.LogError($"Failed to load performer {scene.Id}: {errorMessage}");
					PLogger.LogError(ex.Message);
					PLogger.LogError(ex.StackTrace);
				}
			}

			OnLoadPeformers?.Invoke();

			PLogger.LogInfo("Performers loaded");
		}
	}
}

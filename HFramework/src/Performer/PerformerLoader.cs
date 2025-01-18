#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using HFramework.ConfigFiles;
using YotanModCore;

namespace HFramework.Performer
{
	public class PerformerLoader
	{
		public delegate void LoadPeformers();

		public static event LoadPeformers? OnLoadPeformers;

		public static Dictionary<string, SexPerformerInfo> Performers = new Dictionary<string, SexPerformerInfo>();

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

			var serializer = new XmlSerializer(typeof(PerformersConfig));
			var fileStream = new FileStream("BepInEx/plugins/HFramework/Performers.xml", FileMode.Open);
			var performersConfig = (PerformersConfig)serializer.Deserialize(fileStream);
			fileStream.Close();

			foreach (var performerConfig in performersConfig.Performers)
			{
				string errorMessage = "";
				var builder = new SexPerformerInfoBuilder(performerConfig.Id);

				try
				{
					errorMessage = $"Failed to load Prefab {performerConfig.PrefabSelector?.GetType()}";
					if (performerConfig.PrefabSelector != null)
						builder.SetSexPrefabSelector(performerConfig.PrefabSelector);
					else
						PLogger.LogError($"No PrefabSelector for Performer {performerConfig.Id}");
					
					errorMessage = "Failed to load scopes";
					foreach (var scope in performerConfig.Scopes)
					{
						errorMessage = $"Failed to load Scope {scope}";
						if (scope != PerformerScope.None)
							builder.AddScope(scope);
						else
							PLogger.LogError($"Unknown scope type {scope}");
					}
					
					errorMessage = $"Failed to load Actors";

					int fromNpc = performerConfig.Actors.Length >= 1 ? performerConfig.Actors[0].NpcId : -1;
					int? toNpc = performerConfig.Actors.Length >= 2 ? performerConfig.Actors[1].NpcId : null;

					if (fromNpc != -1)
						builder.SetActors(fromNpc, toNpc);
					else
						PLogger.LogError($"Unknown actora for scene {performerConfig.Id}");

					foreach (var animSet in performerConfig.AnimationSets)
					{
						errorMessage = $"Failed to load Animation Set {animSet.Id}";

						var animSetBuilder = new AnimationSetBuilder(animSet.Id);
						foreach (var anim in animSet.Animations)
						{
							errorMessage = $"Failed to load Animation {anim.Action}";

							if (anim.Action == ActionType.None)
							{
								PLogger.LogError($"Unknown action type {anim.Action} for set {animSet.Id}");
								continue;
							}

							if (anim.Play == PlayType.None)
							{
								PLogger.LogError($"Unknown play type {anim.Play} for set {animSet.Id}");
								continue;
							}

							var pose = anim.Pose;
							var canChangePose = anim.ChangePose;
							animSetBuilder.AddAnimation(anim.Action, pose, new ActionValue(anim.Play, anim.Name, anim.Events, canChangePose));
						}
						builder.AddAnimationSet(animSetBuilder.Build());
					}

					Performers.Add(performerConfig.Id, builder.Build());
				}
				catch (System.Exception ex)
				{
					PLogger.LogError($"Failed to load performer {performerConfig.Id}: {errorMessage}");
					PLogger.LogError(ex.Message);
					PLogger.LogError(ex.StackTrace);
				}
			}
			OnLoadPeformers?.Invoke();

			PLogger.LogInfo("Performers loaded");
		}
	}
}

#nullable enable

using System;
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

		public delegate void RegisterPrefabSelectors();

		public static event RegisterPrefabSelectors? OnRegisterPrefabSelectors;

		private static readonly List<Type> PrefabSelectors = [];

		private static XmlSerializer? Serializer = null;

		public static Dictionary<string, SexPerformerInfo> Performers = [];

		/// <summary>
		/// Performer IDs skipped due to not being available in the current game context
		/// </summary>
		public static List<string> SkippedPerformers = [];

		internal static void RegisterHFPrefabSelectors()
		{
			AddPrefabSelector(typeof(SexListPrefabSelector));
			AddPrefabSelector(typeof(SexObjPrefabSelector));	
		}

		/// <summary>
		/// Reads a performer definition file and returns the parsed data.
		/// It does NOT add it to the game (so you can do whatever you need with it)
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static PerformersConfig ParsePerformersDefinitionFile(string path)
		{
			if (Serializer == null)
			{
				Serializer = new XmlSerializer(typeof(PerformersConfig), PrefabSelectors.ToArray());
				PrefabSelectors.Clear();
			}

			var fileStream = new FileStream(path, FileMode.Open);
			var performersConfig = (PerformersConfig)Serializer.Deserialize(fileStream);
			fileStream.Close();

			return performersConfig;
		}

		/// <summary>
		/// Adds a new type as a possible PrefabSelector that may used in definition files
		/// </summary>
		/// <param name="selector"></param>
		/// <exception cref="Exception"></exception>
		public static void AddPrefabSelector(Type selector)
		{
			if (Serializer != null)
				throw new Exception("The serializer was already built. New prefab selectors can't be added.");

			PrefabSelectors.Add(selector);
		}

		/// <summary>
		/// Adds a single performer config to the game
		/// </summary>
		/// <param name="performerConfig"></param>
		public static void AddPerformerFromConfig(PerformerConfig performerConfig)
		{
			if (performerConfig.DLC && !GameInfo.HasDLC)
			{
				PLogger.LogDebug($"Skipping Performer {performerConfig.Id} because it is only available in DLC mode");
				SkippedPerformers.Add(performerConfig.Id);
				return;
			}

			if (GameInfo.ToVersion(performerConfig.MinVersion ?? "0.0.0") > GameInfo.GameVersion)
			{
				PLogger.LogDebug($"Skipping Performer {performerConfig.Id} because it requires a newer version (minVersion: {performerConfig.MinVersion})");
				SkippedPerformers.Add(performerConfig.Id);
				return;
			}

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

		/// <summary>
		/// Adds all the performers from a performers config to the game
		/// </summary>
		/// <param name="performersConfig"></param>
		public static void AddPerformersFromConfig(PerformersConfig performersConfig)
		{
			foreach (var performerConfig in performersConfig.Performers)
			{
				AddPerformerFromConfig(performerConfig);
			}
		}

		/// <summary>
		/// Loads a performers config file and adds all the performers to the game
		/// </summary>
		/// <param name="path"></param>
		public static void AddPerformersFromFile(string path)
		{
			var performersConfig = ParsePerformersDefinitionFile(path);
			AddPerformersFromConfig(performersConfig);
		}

		/// <summary>
		/// Loads performers
		/// This both loads the HFramework performers and also trigger the event for other plugins
		/// </summary>
		internal static void Load()
		{
			PLogger.LogInfo("Registering Prefab Selectors...");

			OnRegisterPrefabSelectors += PerformerLoader.RegisterHFPrefabSelectors;

			OnRegisterPrefabSelectors?.Invoke();

			PLogger.LogInfo("Loading performers");

			string[] definitions = [
				"AssWall_Performers.xml",
				"CommonSexNPC_Performers.xml",
				"CommonSexPlayer_Performers.xml",
				"Daruma_Performers.xml",
				"Delivery_Performers.xml",
				"ManRapesSleep_Performers.xml",
				"ManRapes_Performers.xml",
				"Onani_Performers.xml",
				"PlayerRaped_Performers.xml",
				"Slave_Performers.xml",
				"Toilet_Performers.xml"
			];

			foreach (var definition in definitions)
			{
				AddPerformersFromFile($"BepInEx/plugins/HFramework/definitions/{definition}");
			}

			OnLoadPeformers?.Invoke();

			PLogger.LogInfo("Performers loaded");
		}
	}
}

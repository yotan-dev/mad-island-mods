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
		public static readonly HashSet<string> MigratedPerformers = new() {
			// AssWall
			"HF_AssWall_Man_FemaleNative",
			"HF_AssWall_Man_NativeGirl",
			"HF_AssWall_Man_FemaleLargeNative",
			"HF_AssWall_Man_UnderGroundWoman",
			"HF_AssWall_Man_ElderSisterNative",

			// CommonSexNpc
			"HF_YoungMan_FemaleNative_Friendly_Normal", // 1-1
			"HF_YoungMan_NativeGirl_Friendly_Normal",// 1-3
			"HF_YoungMan_FemaleNative_Friendly_Pregnant", // 1-20
			"HF_YoungMan_NativeGirl_Friendly_Pregnant", // 1-28
			"HF_MaleNative_FemaleNative_Friendly_Doggy_Normal", // 2-0
			"HF_MaleNative_NativeGirl_Friendly_Normal", // 2-1
			"HF_Keigo_Reika_4_0_Love",// 4-0
			"HF_Takumi_YoungLady_6_1_Love", // 6-1
			"HF_NativeGirl_NativeGirl_Friendly_Normal", // 10-0
			"HF_FemaleNative_FemaleNative_Friendly_Normal", // 15-15

			// CommonSexPlayer / Man
			"HF_Man_Yona_0-2", // 0-2
			"HF_Man_YoungLady_0-2", // 0-2_young
			"HF_Man_Yona_0-3", // 0-3
			"HF_Man_YoungLady_0-3", // 0-3_young
			"HF_Man_Yona_0-4", // 0-4
			"HF_Man_YoungLady_0-4", // 0-4_young
			"HF_Man_FemaleNative_Friendly_Normal", // 1-1
			"HF_Man_NativeGirl_Friendly_Normal", // 1-3
			"HF_Man_Reika_Friendly_RevCowgirl", // 1-12
			"HF_Man_Reika_Friendly_Cowgirl", // 1-17
			"HF_Man_FemaleNative_Friendly_Pregnant", // 1-20
			"HF_Man_Nami_Friendly_ver1", // 1-26_nami
			"HF_Man_SlenderYoungLady_Friendly_ver1", // 1-26b_slender
			"HF_Man_Nami_Friendly_ver2", // 1-27
			"HF_Man_SlenderYoungLady_Friendly_ver2", // 1-27b_slender
			"HF_Man_NativeGirl_Friendly_Pregnant", // 1-28

			// Delivery
			"HF_FemaleNative_Delivery", // FemaleNative
			"HF_NativeGirl_Delivery",// NativeGirl
			"HF_FemaleLargeNative_Delivery", // FemaleLargeNative
			"HF_UnderGroundWoman_Delivery",// UnderGroundWoman
			"HF_Yona_Delivery",// Yona
			"HF_YoungLady_Delivery",// YoungLady
			"HF_Daughter_Delivery", // Daughter
			"HF_UnderGirl_Delivery",// UnderGroundGirl
			"HF_LargeGirl_Delivery", // LargeNativeGirl

			// PlayerRaped / Yona
			"HF_MaleNative_Yona_Rape_Doggy_Normal", // 10
			"HF_BigNative_Yona_Rape", // 11
			"HF_Bigfoot_Yona_Rape", // 25
			"HF_Werewolf_Yona_Rape", // 35
			"HF_Oldguy_Yona_Rape", // 100
			"HF_Spike_Yona_Rape", // 101
			"HF_Planton_Yona_Rape", // 103
			"HF_BossNative_Yona_Rape", // 104
		};

		public delegate void LoadPeformers();

		public static event LoadPeformers? OnLoadPeformers;

		public delegate void RegisterPrefabSelectors();

		public static event RegisterPrefabSelectors? OnRegisterPrefabSelectors;

		private static readonly List<Type> PrefabSelectors = new();

		private static XmlSerializer? Serializer = null;

		public static Dictionary<string, SexPerformerInfo> Performers = new();

		/// <summary>
		/// Performer IDs skipped due to not being available in the current game context
		/// </summary>
		public static List<string> SkippedPerformers = new();

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
				// If Modern mode is enabled, check if this performer is not already migrated to the new system
				// Otherwise we will duplicate records.
				if (HFConfig.Instance.IsModernModeEnabled && MigratedPerformers.Contains(performerConfig.Id)) {
					PLogger.LogDebug($"Modern mode is enabled. Skipping Performer {performerConfig.Id} because it is already migrated to the new system");
					continue;
				}

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

			string[] definitions = new string[] {
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
			};

			foreach (var definition in definitions)
			{
				AddPerformersFromFile($"BepInEx/plugins/HFramework/definitions/{definition}");
			}

			OnLoadPeformers?.Invoke();

			PLogger.LogInfo("Performers loaded");
		}
	}
}

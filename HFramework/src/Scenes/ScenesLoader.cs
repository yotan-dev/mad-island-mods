#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using HFramework.ConfigFiles;
using HFramework.Scenes.Conditionals;

namespace HFramework.Scenes
{
	public static class ScenesLoader
	{
		private static XmlSerializer? Serializer = null;

		private static List<Type> ConditionalTypes = [];

		/// <summary>
		/// Register HFramework-provided conditionals
		/// </summary>
		internal static void RegisterConditionals()
		{
			AddConditional(typeof(FaintCheck));
			AddConditional(typeof(FriendCheck));
			AddConditional(typeof(JokeCheck));
			AddConditional(typeof(LibidoCheck));
			AddConditional(typeof(PerfumeCheck));
			AddConditional(typeof(PregnantCheck));
			AddConditional(typeof(QuestProgressCheck));
			AddConditional(typeof(SexTypeCheck));
		}

		/// <summary>
		/// Registers HFramework scenes
		/// </summary>
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

		/// <summary>
		/// Parses a Scenes definition file and return the data, without adding it to the game.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static ScenesConfig ParseScenesConfigDefinitionFile(string path)
		{
			if (Serializer == null)
			{
				Serializer = new XmlSerializer(typeof(ScenesConfig), ConditionalTypes.ToArray());
				ConditionalTypes.Clear();
			}

			var fileStream = new FileStream(path, FileMode.Open);
			var scenesConfig = (ScenesConfig)Serializer.Deserialize(fileStream);
			fileStream.Close();

			return scenesConfig;
		}

		/// <summary>
		/// Loads a single scene config into the game
		/// </summary>
		/// <param name="scene"></param>
		public static void LoadSceneFromConfig(SceneConfig scene)
		{
			string errorMessage = "";

			try
			{
				PLogger.LogDebug($"Loading info for scene {scene.Id}");
				var sceneInfo = ScenesManager.Instance.GetSceneInfo(scene.Id);
				if (sceneInfo == null)
				{
					PLogger.LogError($"Unknown scene: {scene.Id}");
					return;
				}

				foreach (var performerConfig in scene.Performers)
				{
					var performerConst = performerConfig.Performer;
					var desc = $"(Scene: {scene.Id} / Performer: {performerConst})";
					PLogger.LogDebug($"Loading performer {desc}");

					var performer = Performer.PerformerLoader.Performers.GetValueOrDefault(performerConst);
					if (performer == null)
					{
						if (!Performer.PerformerLoader.SkippedPerformers.Contains(performerConst))
							PLogger.LogError($"Unknown performer: {performerConst} {desc}");
						continue;
					}

					errorMessage = $"Failed to load Performer {performerConst} {desc}";
					sceneInfo.AddPerformer(performer, performerConfig.StartConditions, performerConfig.PerformConditions);
				}
			}
			catch (Exception e)
			{
				PLogger.LogError($"Failed to load scenes: {errorMessage}. {e.Message}");
				PLogger.LogError(e.StackTrace);
			}
		}

		/// <summary>
		/// Loads all the scenes from a scenes config into the game
		/// </summary>
		/// <param name="scenesConfig"></param>
		public static void LoadScenesFromConfig(ScenesConfig scenesConfig)
		{
			foreach (var scene in scenesConfig.Scenes)
			{
				LoadSceneFromConfig(scene);
			}
		}

		/// <summary>
		/// Loads all the scenes from a scenes config file into the game
		/// </summary>
		/// <param name="path"></param>
		public static void LoadScenesFromFile(string path)
		{
			var scenesConfig = ParseScenesConfigDefinitionFile(path);
			LoadScenesFromConfig(scenesConfig);
		}

		/// <summary>
		/// Register a new conditional that may be used by definition files
		/// </summary>
		/// <param name="type"></param>
		/// <exception cref="Exception"></exception>
		public static void AddConditional(Type type)
		{
			if (Serializer != null)
				throw new Exception("The serializer was already built. New conditionals can't be added.");

			ScenesLoader.ConditionalTypes.Add(type);
		}

		/// <summary>
		/// Register the framework provided performers
		/// </summary>
		internal static void RegisterScenePerformers()
		{
			try
			{
				string[] definitions = [
					"AssWall_Scenes.xml",
					"CommonSexNPC_Scenes.xml",
					"CommonSexPlayer_Scenes.xml",
					"Daruma_Scenes.xml",
					"Delivery_Scenes.xml",
					"ManRapesSleep_Scenes.xml",
					"ManRapes_Scenes.xml",
					"Onani_Scenes.xml",
					"PlayerRaped_Scenes.xml",
					"Slave_Scenes.xml",
					"Toilet_Scenes.xml",
				];

				foreach (var definition in definitions)
					LoadScenesFromFile($"BepInEx/plugins/HFramework/definitions/{definition}");
			}
			catch (Exception e)
			{
				PLogger.LogError($"Failed to load scenes. {e.Message}");
				PLogger.LogError(e.StackTrace);
			}
		}
	}
}

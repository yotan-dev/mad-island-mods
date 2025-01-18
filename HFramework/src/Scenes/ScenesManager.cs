#nullable enable

using System.Collections.Generic;
using HFramework.Performer;

namespace HFramework.Scenes
{
	public class ScenesManager
	{
		public delegate void RegisterScenes();

		public static event RegisterScenes? OnRegisterScenes;

		public delegate void RegisterScenePerformers();

		public static event RegisterScenePerformers? OnRegisterScenePerformers;

		public delegate void RegisterConditionals();

		public static event RegisterConditionals? OnRegisterConditionals;

		public static ScenesManager Instance = new ScenesManager();

		private Dictionary<string, SceneInfo> SceneInfos = [];

		private ScenesManager() { }

		internal void Init()
		{
			// Always load HF system first, so modders can change them.

			PLogger.LogInfo("Registering conditionals...");
			ScenesLoader.RegisterConditionals();
			OnRegisterConditionals?.Invoke();

			PLogger.LogInfo("Registering scenes...");
			ScenesLoader.RegisterScenes();
			OnRegisterScenes?.Invoke();

			PLogger.LogInfo("Loading scenes...");
			ScenesLoader.RegisterScenePerformers();
			OnRegisterScenePerformers?.Invoke();

			PLogger.LogInfo("Scenes loaded.");
		}

		public void AddScene(string name, SceneInfo info)
		{
			SceneInfos.Add(name, info);
		}

		public SceneInfo? GetSceneInfo(string name)
		{
			return SceneInfos.GetValueOrDefault(name);
		}

		public SexPerformer? GetPerformer(IScene scene, PerformerScope scope, ISceneController controller)
		{
			var actors = scene.GetActors();
			if (actors.Length == 0)
				return null;

			var from = actors[0];
			var to = actors.Length > 1 ? actors[1] : null;
			var performer = this.GetSceneInfo(scene.GetName())
				?.GetPerformerInfo(scene, scope, from.npcID, to?.npcID);

			if (performer == null)
				return null;

			return new SexPerformer(performer, controller);
		}
	}
}

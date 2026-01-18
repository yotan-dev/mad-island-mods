#nullable enable

using System.Collections.Generic;
using HFramework.Performer;
using HFramework.Scenes.Conditionals;
using YotanModCore;

namespace HFramework.Scenes
{
	public class SceneInfo
	{
		protected class ScenePerformer
		{
			public SexPerformerInfo Performer { get; private set; }

			public IConditional[] StartConditions { get; private set; }

			public IConditional[] PerformConditions { get; private set; }

			public ScenePerformer(SexPerformerInfo performer, IConditional[] startConditions, IConditional[] performConditions)
			{
				this.Performer = performer;
				this.StartConditions = startConditions;
				this.PerformConditions = performConditions;
			}

			public bool CanStart(PerformerScope scope, CommonStates from, CommonStates? to)
			{
				if (!this.Performer.Scopes.Contains(scope))
					return false;

				foreach (var conditional in this.StartConditions)
				{
					// We must check it here instead of inside QuestProgressCheck, because QuestProgressCheck
					// may be used for CanPerform too -- and currently we don't want that check there
					// to match the original code.
					if (conditional is QuestProgressCheck && GameInfo.RemoveQuestConditionForSex) {
						PLogger.LogConditionDebug($"CanStart({this.Performer.Id})[{conditional.GetType().Name}]: {from.charaName} -> {to?.charaName} - Skipped due to Quest bypass");
						continue;
					}

					var pass = conditional.Pass(from, to);
					PLogger.LogConditionDebug($"CanStart({this.Performer.Id})[{conditional.GetType().Name}]: {from.charaName} -> {to?.charaName} = {pass}");
					if (!pass)
						return false;
				}

				return true;
			}

			public bool CanPerform(IScene scene, PerformerScope scope)
			{
				if (!this.Performer.Scopes.Contains(scope))
					return false;

				foreach (var conditional in this.PerformConditions)
				{
					var pass = conditional.Pass(scene);
					PLogger.LogConditionDebug($"CanPerform({this.Performer.Id})[{conditional.GetType().Name}]: {scene.GetName()} = {pass}");
					if (!pass)
						return false;
				}

				return true;
			}
		}

		public string Name { get; private set; }

		protected Dictionary<int, Dictionary<int, List<ScenePerformer>>> Performers = [];

		protected Dictionary<string, ScenePerformer> IdToPerformer = [];

		public SceneInfo(string name)
		{
			this.Name = name;
		}

		public void AddPerformer(SexPerformerInfo performer, IConditional[] StartConditions, IConditional[] PerformConditions)
		{
			Dictionary<int, List<ScenePerformer>> toPerformerList;
			if (!Performers.TryGetValue(performer.FromNpcId, out toPerformerList))
			{
				toPerformerList = new Dictionary<int, List<ScenePerformer>>();
				Performers.Add(performer.FromNpcId, toPerformerList);
			}

			List<ScenePerformer> performerList;
			if (!toPerformerList.TryGetValue(performer.ToNpcId ?? -1, out performerList))
			{
				performerList = new List<ScenePerformer>();
				toPerformerList.Add(performer.ToNpcId ?? -1, performerList);
			}

			var scnPerformer = new ScenePerformer(performer, StartConditions, PerformConditions);
			performerList.Add(scnPerformer);
			if (!IdToPerformer.ContainsKey(performer.Id))
				IdToPerformer.Add(performer.Id, scnPerformer);
		}

		public bool CanStart(PerformerScope scope, CommonStates from, CommonStates? to)
		{
			if (Performers.TryGetValue(from.npcID, out var toPerformerList))
			{
				if (toPerformerList.TryGetValue(to?.npcID ?? -1, out var performerList))
				{
					foreach (var performer in performerList)
					{
						PLogger.LogConditionDebug($"CanStart({performer.Performer.Id}): {from.charaName} -> {to?.charaName}");
						if (performer.CanStart(scope, from, to))
							return true;
					}

				}
			}

			return false;
		}

		public SexPerformerInfo? GetPerformerInfo(IScene scene, PerformerScope scope, int fromNpcId, int? toNpcId = null)
		{
			if (Performers.TryGetValue(fromNpcId, out var toPerformerList))
			{
				if (toPerformerList.TryGetValue(toNpcId ?? -1, out var performerList))
				{
					foreach (var performer in performerList)
					{
						if (performer.CanPerform(scene, scope))
							return performer.Performer;
					}

				}
			}

			return null;
		}

		public SexPerformerInfo? GetPerformerById(string performerId)
		{
			if (IdToPerformer.ContainsKey(performerId))
				return IdToPerformer[performerId].Performer;

			return null;
		}
	}
}

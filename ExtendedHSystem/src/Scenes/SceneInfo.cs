using System.Collections.Generic;
using ExtendedHSystem.Performer;

namespace ExtendedHSystem.Scenes
{
	public class SceneInfo
	{
		public string Name { get; private set; }

		protected static Dictionary<int, Dictionary<int?, List<SexPerformerInfo>>> Performers = [];

		public SceneInfo(string name)
		{
			this.Name = name;
		}

		public void AddPerformer(SexPerformerInfo performer)
		{
			Dictionary<int?, List<SexPerformerInfo>> toPerformerList;
			if (!Performers.TryGetValue(performer.FromNpcId, out toPerformerList))
			{
				toPerformerList = new Dictionary<int?, List<SexPerformerInfo>>();
				Performers.Add(performer.FromNpcId, toPerformerList);
			}

			List<SexPerformerInfo> performerList;
			if (!toPerformerList.TryGetValue(performer.ToNpcId, out performerList))
			{
				performerList = new List<SexPerformerInfo>();
				toPerformerList.Add(performer.ToNpcId, performerList);
			}

			performerList.Add(performer);
		}

		public SexPerformerInfo GetPerformerInfo(IScene2 scene, PerformerScope scope, int fromNpcId, int? toNpcId = null)
		{
			if (Performers.TryGetValue(fromNpcId, out var toPerformerList))
			{
				if (toPerformerList.TryGetValue(toNpcId, out var performerList))
				{
					foreach (var performer in performerList)
					{
						if (performer.CanPlay(scene, scope))
							return performer;
					}

				}
			}

			return null;
		}
	}
}

using System.Collections.Generic;

namespace ExtendedHSystem.Performer
{
	public class SexPerformerInfo
	{
		public static readonly string DefaultSet = "Default";

		public static readonly string AnimSetChangeName = "@AnimSetChange";

		public readonly string Id;

		public readonly IPrefabSelector SexPrefabSelector;

		public readonly int FromNpcId;

		public readonly int? ToNpcId;

		public readonly Dictionary<string, AnimationSet> AnimationSets = [];

		public readonly List<PerformerScope> Scopes;

		public SexPerformerInfo(
			string id,
			int from,
			int? to,
			IPrefabSelector sexPrefabSelector,
			Dictionary<string, AnimationSet> animationSets,
			List<PerformerScope> scopes
		)
		{
			this.Id = id;
			this.FromNpcId = from;
			this.ToNpcId = to;
			this.SexPrefabSelector = sexPrefabSelector;
			this.AnimationSets = animationSets;
			this.Scopes = scopes;
		}
	}
}

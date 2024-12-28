using System.Collections.Generic;

namespace ExtendedHSystem.Performer
{
	public class SexPerformerInfo
	{
		public readonly string Id;
		public readonly IPrefabSelector SexPrefabSelector;

		public readonly int FromNpcId;

		public readonly int? ToNpcId;

		public readonly Dictionary<ActionKey, ActionValue> Actions = [];

		public readonly List<PerformerScope> Scopes;

		public SexPerformerInfo(
			string id,
			int from,
			int? to,
			IPrefabSelector sexPrefabSelector,
			Dictionary<ActionKey, ActionValue> actions,
			List<PerformerScope> scopes
		)
		{
			this.Id = id;
			this.FromNpcId = from;
			this.ToNpcId = to;
			this.SexPrefabSelector = sexPrefabSelector;
			this.Actions = actions;
			this.Scopes = scopes;
		}
	}
}

using System.Collections.Generic;
using ExtendedHSystem.Scenes;

namespace ExtendedHSystem.Performer
{
	public class SexPerformerInfo
	{
		public readonly string Id;
		public readonly IPrefabSelector SexPrefabSelector;

		public readonly int FromNpcId;

		public readonly int? ToNpcId;

		public readonly List<IConditional> Conditionals;

		public readonly Dictionary<ActionKey, ActionValue> Actions = [];

		public readonly List<PerformerScope> Scopes;

		public SexPerformerInfo(
			string id,
			int from,
			int? to,
			IPrefabSelector sexPrefabSelector,
			List<IConditional> conditionals,
			Dictionary<ActionKey, ActionValue> actions,
			List<PerformerScope> scopes
		)
		{
			this.Id = id;
			this.FromNpcId = from;
			this.ToNpcId = to;
			this.SexPrefabSelector = sexPrefabSelector;
			this.Conditionals = conditionals;
			this.Actions = actions;
			this.Scopes = scopes;
		}

		public bool CanPlay(IScene2 scene, PerformerScope scope)
		{
			if (!this.Scopes.Contains(scope))
				return false;

			foreach (var conditional in this.Conditionals)
			{
				if (!conditional.Pass(scene))
					return false;
			}

			return true;
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtendedHSystem.Performer
{
	public class SexPerformerInfoBuilder
	{
		private string Id { get; set; }
		private IPrefabSelector SexPrefabSeletor { get; set; }

		private int FromNpcId { get; set; }

		private int? ToNpcId { get; set; }

		private List<IConditional> Conditionals { get; set; } = [];

		private Dictionary<ActionKey, ActionValue> Actions { get; set; } = [];

		public SexPerformerInfoBuilder(string id)
		{
			this.Id = id;
		}

		public SexPerformerInfoBuilder SetActors(int fromNpcId, int? toNpcId = null)
		{
			this.FromNpcId = fromNpcId;
			this.ToNpcId = toNpcId;
			return this;
		}

		public SexPerformerInfoBuilder SetSexPrefabSelector(IPrefabSelector selector)
		{
			this.SexPrefabSeletor = selector;
			return this;
		}

		public SexPerformerInfoBuilder AddCondition(IConditional condition)
		{
			this.Conditionals.Add(condition);
			return this;
		}

		public SexPerformerInfoBuilder AddAnimation(ActionType actionType, int pose, ActionValue value)
		{
			this.Actions.Add(new ActionKey(actionType, pose), value);
			return this;
		}

		public SexPerformerInfoBuilder AddAnimation(ActionType actionType, ActionValue value)
		{
			this.AddAnimation(actionType, 1, value);
			return this;
		}

		public SexPerformerInfo Build()
		{
			return new SexPerformerInfo(this.FromNpcId, this.ToNpcId, this.SexPrefabSeletor, this.Conditionals, this.Actions);
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtendedHSystem.Performer
{
	public class SexPerformerInfoBuilder
	{
		public string Id { get; set; }
		public GameObject SexPrefab { get; set; }

		public int FromNpcId { get; set; }

		public int? ToNpcId { get; set; }

		public List<IConditional> Conditionals { get; set; } = [];

		public Dictionary<ActionType, Func<ISceneController, IEnumerator>> Actions { get; set; } = [];

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

		public SexPerformerInfoBuilder SetSexPrefab(GameObject prefab)
		{
			this.SexPrefab = prefab;
			return this;
		}

		public SexPerformerInfoBuilder AddCondition(IConditional condition)
		{
			this.Conditionals.Add(condition);
			return this;
		}

		public SexPerformerInfoBuilder AddAnimation(ActionType actionType, Func<ISceneController, IEnumerator> executor)
		{
			this.Actions.Add(actionType, executor);
			return this;
		}

		public SexPerformerInfo Build()
		{
			return new SexPerformerInfo(this.FromNpcId, this.ToNpcId, this.SexPrefab, this.Conditionals, this.Actions);
		}
	}
}
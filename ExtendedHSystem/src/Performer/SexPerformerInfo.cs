using System;
using System.Collections;
using System.Collections.Generic;
using ExtendedHSystem.Scenes;
using UnityEngine;

namespace ExtendedHSystem.Performer
{
	public class SexPerformerInfo
	{
		public readonly GameObject SexPrefab;

		public readonly int FromNpcId;

		public readonly int? ToNpcId;

		public readonly List<IConditional> Conditionals;

		public readonly Dictionary<ActionKey, ActionValue> Actions = [];

		public SexPerformerInfo(
			int from,
			int? to,
			GameObject sexPrefab,
			List<IConditional> conditionals,
			Dictionary<ActionKey, ActionValue> actions
		)
		{
			this.FromNpcId = from;
			this.ToNpcId = to;
			this.SexPrefab = sexPrefab;
			this.Conditionals = conditionals;
			this.Actions = actions;
		}

		public bool CanPlay(IScene2 scene)
		{
			foreach (var conditional in this.Conditionals)
			{
				if (!conditional.Pass(scene))
					return false;
			}

			return true;
		}
	}
}

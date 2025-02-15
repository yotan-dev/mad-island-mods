using System.Collections.Generic;

namespace HFramework.Performer
{
	public class SexPerformerInfoBuilder
	{
		private string Id { get; set; }
		private IPrefabSelector SexPrefabSeletor { get; set; }

		private int FromNpcId { get; set; }

		private int? ToNpcId { get; set; }

		private List<PerformerScope> Scopes { get; set; } = [];

		private Dictionary<string, AnimationSet> AnimationSets { get; set; } = [];

		public SexPerformerInfoBuilder(string id)
		{
			this.Id = id;
		}

		public SexPerformerInfoBuilder AddScope(PerformerScope scope)
		{
			this.Scopes.Add(scope);
			return this;
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
		
		public SexPerformerInfoBuilder AddAnimationSet(AnimationSet animationSet)
		{
			this.AnimationSets.Add(animationSet.Name, animationSet);
			return this;
		}

		public SexPerformerInfo Build()
		{
			return new SexPerformerInfo(this.Id, this.FromNpcId, this.ToNpcId, this.SexPrefabSeletor, this.AnimationSets, this.Scopes);
		}
	}
}

using System.Collections.Generic;

namespace HFramework.ConfigFiles
{
	public class PerformerConfig
	{
		public string Id { get; set; }

		public string[] Scopes { get; set; }

		public PrefabConfig Prefab { get; set; }

		public string[] Actors { get; set; }

		public Dictionary<string, AnimationsConfig[]> AnimationSets { get; set; }
	}
}

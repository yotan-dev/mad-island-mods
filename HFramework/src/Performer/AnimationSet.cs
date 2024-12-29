using System.Collections.Generic;

namespace HFramework.Performer
{
	public class AnimationSet
	{
		public readonly string Name;
		
		public Dictionary<ActionKey, ActionValue> Actions { get; private set; } = [];

		public AnimationSet(string name)
		{
			this.Name = name;
		}
	}
}

namespace ExtendedHSystem.Performer
{
	public class AnimationSetBuilder
	{
		private AnimationSet AnimationSet;

		public AnimationSetBuilder(string name)
		{
			this.AnimationSet = new AnimationSet(name);
		}

		public AnimationSetBuilder AddAnimation(ActionType actionType, int pose, ActionValue value)
		{
			this.AnimationSet.Actions.Add(new ActionKey(actionType, pose), value);
			return this;
		}

		public AnimationSetBuilder AddAnimation(ActionType actionType, ActionValue value)
		{
			this.AddAnimation(actionType, 1, value);
			return this;
		}

		public AnimationSet Build()
		{
			return this.AnimationSet;
		}
	}
}

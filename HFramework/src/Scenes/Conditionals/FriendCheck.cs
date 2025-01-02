namespace HFramework.Scenes.Conditionals
{
	public class FriendCheck : IConditional
	{
		public int NpcId { get; private set; }
		public bool ExpectedValue { get; private set; }


		public FriendCheck(int npcId, bool expectedValue)
		{
			this.NpcId = npcId;
			this.ExpectedValue = expectedValue;
		}

		public bool Pass(IScene2 scene)
		{
			bool isFriend = true;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					isFriend = isFriend && actor.Employed(false);
			}

			return isFriend == this.ExpectedValue;
		}

		public bool Pass(CommonStates from, CommonStates to)
		{
			bool isFriend = true;
			if (from.npcID == this.NpcId)
				isFriend = isFriend && from.Employed(false);

			if (to?.npcID == this.NpcId)
				isFriend = isFriend && to.Employed(false);

			return isFriend == this.ExpectedValue;
		}
	}
}

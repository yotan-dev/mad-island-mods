namespace HFramework.Scenes.Conditionals
{
	public class FaintCheck : IConditional
	{
		public int NpcId { get; private set; }
		public bool ExpectedValue { get; private set; }


		public FaintCheck(int npcId, bool expectedValue)
		{
			this.NpcId = npcId;
			this.ExpectedValue = expectedValue;
		}

		public bool Pass(IScene2 scene)
		{
			bool isFainted = true;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					isFainted = isFainted && (actor.faint <= 0 || actor.life <= 0);
			}

			return isFainted == this.ExpectedValue;
		}

		public bool Pass(CommonStates from, CommonStates to)
		{
			bool pass = true;
			if (from.npcID == this.NpcId)
				pass = pass && (from.faint <= 0 || from.life <= 0);

			if (to?.npcID == this.NpcId)
				pass = pass && (to.faint <= 0 || to.life <= 0);

			return pass == this.ExpectedValue;
		}
	}
}

namespace HFramework.Scenes.Conditionals
{
	public class PerfumeCheck : IConditional
	{
		public int NpcId { get; private set; }
		public bool ExpectedValue { get; private set; }


		public PerfumeCheck(int npcId, bool expectedValue)
		{
			this.NpcId = npcId;
			this.ExpectedValue = expectedValue;
		}

		public bool Pass(IScene2 scene)
		{
			bool hasPerfume = false;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					hasPerfume = hasPerfume || actor.debuff.perfume > 0f;
			}

			return hasPerfume == this.ExpectedValue;
		}

		public bool Pass(CommonStates from, CommonStates to)
		{
			bool hasPerfume = false;
			if (from.npcID == this.NpcId)
				hasPerfume = hasPerfume || (from.debuff.perfume > 0f);

			if (to?.npcID == this.NpcId)
				hasPerfume = hasPerfume || (to.debuff.perfume > 0f);

			return hasPerfume == this.ExpectedValue;
		}
	}
}

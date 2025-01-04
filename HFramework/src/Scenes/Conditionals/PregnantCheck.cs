using YotanModCore;

namespace HFramework.Scenes.Conditionals
{
	public class PregnantCheck : IConditional
	{
		public int NpcId { get; private set; }
		public bool ExpectedValue { get; private set; }

		public PregnantCheck(int npcId, bool expectedValue)
		{
			this.NpcId = npcId;
			this.ExpectedValue = expectedValue;
		}

		public bool Pass(IScene scene)
		{
			bool isPregnant = false;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					isPregnant = isPregnant || CommonUtils.IsPregnant(actor);
			}
			
			return isPregnant == this.ExpectedValue;
		}

		public bool Pass(CommonStates from, CommonStates to)
		{
			bool pass = true;
			if (from.npcID == this.NpcId)
				pass = pass && CommonUtils.IsPregnant(from) == this.ExpectedValue;

			if (to?.npcID == this.NpcId)
				pass = pass && CommonUtils.IsPregnant(to) == this.ExpectedValue;

			return pass;
		}
	}
}

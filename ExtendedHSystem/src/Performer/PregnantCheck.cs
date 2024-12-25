using ExtendedHSystem.Scenes;
using YotanModCore;

namespace ExtendedHSystem.Performer
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

		public bool Pass(IScene2 scene)
		{
			bool isPregnant = false;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					isPregnant = isPregnant || CommonUtils.IsPregnant(actor);
			}
			
			return isPregnant == this.ExpectedValue;
		}
	}
}

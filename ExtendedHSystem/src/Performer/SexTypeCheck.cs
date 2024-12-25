using ExtendedHSystem.Scenes;

namespace ExtendedHSystem.Performer
{
	public class SexTypeCheck : IConditional
	{
		public int ExpectedValue { get; private set; }

		public SexTypeCheck(int expectedValue)
		{
			this.ExpectedValue = expectedValue;
		}

		public bool Pass(IScene2 scene)
		{
			if (scene is not CommonSexPlayer commonSexPlayer)
				return false;

			return commonSexPlayer.Type == this.ExpectedValue;
		}
	}
}

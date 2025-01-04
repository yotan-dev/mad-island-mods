using YotanModCore;

namespace HFramework.Scenes.Conditionals
{
	public class JokeCheck : IConditional
	{
		public bool ExpectedValue { get; private set; }


		public JokeCheck(bool expectedValue)
		{
			this.ExpectedValue = expectedValue;
		}

		public bool Pass(IScene scene)
		{
			return GameInfo.JokeMode == this.ExpectedValue;
		}

		public bool Pass(CommonStates from, CommonStates to)
		{
			return GameInfo.JokeMode == this.ExpectedValue;
		}
	}
}

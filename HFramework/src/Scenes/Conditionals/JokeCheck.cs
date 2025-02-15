using System.Xml.Serialization;
using YotanModCore;

namespace HFramework.Scenes.Conditionals
{
	public class JokeCheck : BaseConditional, IConditional
	{
		[XmlAttribute("value")]
		public bool ExpectedValue { get; set; } = false;

		public JokeCheck() {}

		public JokeCheck(bool expectedValue)
		{
			this.ExpectedValue = expectedValue;
		}

		public override bool Pass(IScene scene)
		{
			return GameInfo.JokeMode == this.ExpectedValue;
		}

		public override bool Pass(CommonStates from, CommonStates to)
		{
			return GameInfo.JokeMode == this.ExpectedValue;
		}
	}
}

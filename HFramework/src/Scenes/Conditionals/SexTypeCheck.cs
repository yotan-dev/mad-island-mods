using System.Xml.Serialization;

namespace HFramework.Scenes.Conditionals
{
	public class SexTypeCheck : BaseConditional, IConditional
	{
		[XmlAttribute("value")]
		public int ExpectedValue { get; set; } = 0;

		public SexTypeCheck() { }

		public SexTypeCheck(int expectedValue)
		{
			this.ExpectedValue = expectedValue;
		}

		public override bool Pass(IScene scene)
		{
			if (scene is not CommonSexPlayer commonSexPlayer)
				return false;

			return commonSexPlayer.Type == this.ExpectedValue;
		}

		public override bool Pass(CommonStates from, CommonStates to)
		{
			throw new System.NotImplementedException("SexTypeCheck can't be used outside a scene");
		}
	}
}

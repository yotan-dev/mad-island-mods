using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using YotanModCore;

namespace HFramework.Scenes.Conditionals
{
	public class QuestProgressCheck : BaseConditional, IConditional
	{
		[XmlAttribute("questName")]
		public string QuestName { get; set; } = "";

		[XmlAttribute("compare")]
		public string Compare { get; set; } = "eq";

		[XmlAttribute("value")]
		[DefaultValue(0)]
		public int ExpectedValue { get; set; } = 0;

		[XmlArray("Values")]
		[XmlArrayItem("Value")]
		public int[] ExpectedValues { get; set; } = [];

		public QuestProgressCheck() { }

		public QuestProgressCheck(string questName, string compare, int expectedValue)
		{
			this.QuestName = questName;
			this.Compare = compare;
			this.ExpectedValue = expectedValue;
		}

		public QuestProgressCheck(string questName, string compare, int[] expectedValues)
		{
			this.QuestName = questName;
			this.Compare = compare;
			this.ExpectedValues = expectedValues;
		}

		private bool Pass()
		{
			var progress = Managers.mn.story.QuestProgress(this.QuestName);
			if (this.Compare == "eq")
				return progress == this.ExpectedValue;
			else if (this.Compare == "neq")
				return progress != this.ExpectedValue;
			else if (this.Compare == "gte")
				return progress >= this.ExpectedValue;
			else if (this.Compare == "lte")
				return progress <= this.ExpectedValue;
			else if (this.Compare == "in")
				return this.ExpectedValues.Contains(progress);
			else if (this.Compare == "nin")
				return !this.ExpectedValues.Contains(progress);

			PLogger.LogError($"Unknown compare operator {this.Compare}");
			return false;
		}

		public override bool Pass(IScene scene)
		{
			return this.Pass();
		}

		public override bool Pass(CommonStates from, CommonStates to)
		{
			return this.Pass();
		}
	}
}

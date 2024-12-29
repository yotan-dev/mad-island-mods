using System.Linq;
using YotanModCore;

namespace HFramework.Scenes.Conditionals
{
	public class QuestProgressCheck : IConditional
	{
		public string QuestName { get; private set; }
		public string Compare { get; private set; }
		public int? ExpectedValue { get; private set; } = null;
		public int[] ExpectedValues { get; private set; } = [];


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
			if (this.Compare == "==")
				return progress == this.ExpectedValue.GetValueOrDefault(0);
			else if (this.Compare == "!=")
				return progress != this.ExpectedValue.GetValueOrDefault(0);
			else if (this.Compare == ">=")
				return progress >= this.ExpectedValue.GetValueOrDefault(0);
			else if (this.Compare == "<=")
				return progress <= this.ExpectedValue.GetValueOrDefault(0);
			else if (this.Compare == "in")
				return this.ExpectedValues.Contains(progress);

			PLogger.LogError($"Unknown compare operator {this.Compare}");
			return false;
		}

		public bool Pass(IScene2 scene)
		{
			return this.Pass();
		}

		public bool Pass(CommonStates from, CommonStates to)
		{
			return this.Pass();
		}
	}
}

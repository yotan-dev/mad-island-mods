using ExtendedHSystem.Scenes;
using YotanModCore;

namespace ExtendedHSystem.Performer
{
	public class QuestProgressCheck : IConditional
	{
		public string QuestName { get; private set; }
		public string Compare { get; private set; }
		public int ExpectedValue { get; private set; }

		public QuestProgressCheck(string questName, string compare, int expectedValue)
		{
			this.QuestName = questName;
			this.Compare = compare;
			this.ExpectedValue = expectedValue;
		}

		public bool Pass(IScene2 scene)
		{
			var progress = Managers.mn.story.QuestProgress(this.QuestName);
			if (this.Compare == "==")
				return progress == this.ExpectedValue;
			else if (this.Compare == "!=")
				return progress != this.ExpectedValue;
			else if (this.Compare == ">=")
				return progress >= this.ExpectedValue;
			else if (this.Compare == "<=")
				return progress <= this.ExpectedValue;
			
			PLogger.LogError($"Unknown compare operator {this.Compare}");
			return false;
		}
	}
}

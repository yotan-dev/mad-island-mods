using System;
using System.Linq;
using YotanModCore;

namespace HFramework.SexScripts.Info.Conditions
{
	[Serializable]
	[Experimental]
	public class QuestProgress : Condition
	{
		public string QuestName;

		public int[] QuestValues;

		public override bool CanStart() {
			var progress = Managers.mn.story.QuestProgress(this.QuestName);
			return this.QuestValues.Contains(progress);
		}

		public override bool CanExecute(SexInfo info) {
			var progress = Managers.mn.story.QuestProgress(this.QuestName);
			return this.QuestValues.Contains(progress);
		}
	}
}

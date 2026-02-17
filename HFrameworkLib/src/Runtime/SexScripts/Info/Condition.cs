using System;
using System.Linq;
using HFramework.Tree;
using YotanModCore;

namespace HFramework.SexScripts.Info
{
	[Serializable]
	public class Condition
	{
		public ConditionType Type;
		public string QuestName;
		public int[] QuestValues;
		public int[] SexTypeValues;

		public bool Pass()
		{
			switch (this.Type)
			{
				case ConditionType.QuestProgress:
					{
						var progress = Managers.mn.story.QuestProgress(this.QuestName);
						return this.QuestValues.Contains(progress);
					}

				case ConditionType.SexType:
					PLogger.LogError("SexType condition not supported in Start Condition -- We don't have the target place yet!");
					return false;
			}

			return false;
		}

		public bool Pass(SexInfo info)
		{
			switch (this.Type)
			{
				case ConditionType.QuestProgress:
					{
						var progress = Managers.mn.story.QuestProgress(this.QuestName);
						return this.QuestValues.Contains(progress);
					}

				case ConditionType.SexType:
					if (info is not IHasSexType hasSexType)
					{
						PLogger.LogError("SexType condition not supported in Pass(SexInfo) -- SexInfo does not implement IHasSexType!");
						return false;
					}

					return this.SexTypeValues.Contains(hasSexType.SexType);
			}

			return false;
		}
	}
}

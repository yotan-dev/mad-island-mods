using System.Linq;
using YotanModCore;

namespace ExtendedHSystem
{
	public static class Utils
	{
		public static CommonStates[] SortActors(params CommonStates[] actors)
		{
			return actors.OrderBy(actor =>
			{
				int val = 0;
				if (CommonUtils.IsMale(actor))
					val = 10000;
				else if (CommonUtils.IsFemale(actor))
					val = 20000;
				else
					val = 30000;

				return val + (actor?.npcID ?? 9999);
			}).ToArray();
		}
	}
}

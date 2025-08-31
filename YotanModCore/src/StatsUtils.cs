using UnityEngine;
using UnityEngine.Assertions;
using YotanModCore.Consts;

namespace YotanModCore
{
	public static class StatsUtils
	{
		/// <summary>
		/// Returns the number of Stats points gained in level
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public static int CalculateStatsGain(int level)
		{
			Assert.IsTrue(level >= 0);

			return Mathf.FloorToInt(level / 10f) + 1;
		}

		/// <summary>
		/// Returns the StatsData object for common. StatsData contains info about NPC initial stats and growth
		/// </summary>
		/// <param name="common"></param>
		/// <returns></returns>
		public static StatsData GetStatsData(CommonStates common)
		{
			return Managers.mn.npcMN.GetStatsData(common.npcID);
		}

		/// <summary>
		/// Gets the cost to increase stat for common.
		/// See Stat constant for values
		/// </summary>
		/// <param name="common"></param>
		/// <param name="stat"></param>
		/// <returns></returns>
		public static int GetStatsUpCost(CommonStates common, int stat)
		{
			Assert.IsTrue(stat >= Stat.Min && stat <= Stat.Max);

			return Managers.mn.skillMN.CostCheckStatus(common, stat);
		}

		/// <summary>
		/// Tries to increase "stat" of "common" by 1
		/// </summary>
		/// <param name="common"></param>
		/// <param name="stat"></param>
		public static void StatsUp(CommonStates common, int stat)
		{
			Assert.IsTrue(stat >= Stat.Min && stat <= Stat.Max);

			int cost = GetStatsUpCost(common, stat);
			if (common.statusPoint >= cost)
			{
				var statsData = GetStatsData(common);
				common.statusPoint -= cost;
				common.status[stat]++;

				switch (stat)
				{
					case Stat.Health: common.maxLife += statsData.upLife; break;
					case Stat.Attack: common.attack += statsData.upAttack; break;
					case Stat.Agility: common.speed += 0.005f; break;
				}
			}
		}
	}
}

using EnhancedIsland.NpcStats.Config;
using YotanModCore;
using YotanModCore.Consts;

namespace EnhancedIsland.NpcStats
{
	public class StatDistributor
	{
		private static void DefaultDistributeStats(CommonStates common, StatsData statsData)
		{
			common.maxLife += common.level * statsData.upLife;
			common.attack += common.level * statsData.upAttack;
			common.maxFaint += common.level * statsData.upFaint;
		}

		private static void RandomDistributeStats(CommonStates common)
		{
			bool canIncrease = true;
			while (canIncrease)
			{
				int healthCost = StatsUtils.GetStatsUpCost(common, Stat.Health);
				int atkCost = StatsUtils.GetStatsUpCost(common, Stat.Attack);
				int speedCost = StatsUtils.GetStatsUpCost(common, Stat.Agility);

				long points = common.statusPoint;
				canIncrease = healthCost <= points || atkCost <= points || speedCost <= points;
				if (canIncrease)
				{
					int stats = UnityEngine.Random.Range(0, 300) % 3;
					StatsUtils.StatsUp(common, stats);
				}
			}
		}

		public static void RedistributeStats(CommonStates common, DistributionMode mode, bool extraStrong)
		{
			// Nothing to do here.
			if (mode == DistributionMode.Keep)
				return;

			var statsData = StatsUtils.GetStatsData(common);
			common.maxLife = statsData.life;
			common.attack = statsData.attack;
			common.maxFaint = statsData.faint;

			for (int i = 0; i < Stat.Max; i++)
				common.status[i] = 0;

			if (mode == DistributionMode.ForceLevel1)
			{
				common.level = 1;
				common.statusPoint = 0;
				common.life = common.maxLife;
				common.faint = common.maxFaint;

				return;
			}

			for (int i = 1; i <= common.level; i++)
			{
				common.statusPoint += StatsUtils.CalculateStatsGain(i);
			}

			if (extraStrong)
			{
				// The original game code does the distribution like that.
				// When NpcStats was first made, I misunderstood the original code and ended up including this
				// into the Random Distribution logic. It is now fixed, but let's keep this option here in case someone wants it :)
				DefaultDistributeStats(common, statsData);
			}

			switch (mode)
			{
				case DistributionMode.Default:
					StatDistributor.DefaultDistributeStats(common, statsData);
					break;
				case DistributionMode.Random:
					StatDistributor.RandomDistributeStats(common);
					break;
				case DistributionMode.ForceLevel1:
					/* Already handled above */
					break;
				default:
					break;
			}

			common.life = common.maxLife;
			common.faint = common.maxFaint;
		}

		public static void RedistributeStats(NpcKind kind, CommonStates common)
		{
			switch (kind)
			{
				case NpcKind.Enemy:
					{
						DistributionMode mode = Config.PConfig.Instance.EnemiesDistribution.Value;
						if (mode == DistributionMode.ForceLevel1 || mode == DistributionMode.Keep)
						{
							PLogger.LogError($"RedistributeStats: {mode} is not supported for enemies. Swithcing to random");
							mode = DistributionMode.Random;
						}

						StatDistributor.RedistributeStats(common, mode, Config.PConfig.Instance.ExtraStrongEnemies.Value);
					}
					break;

				case NpcKind.Tamed:
					StatDistributor.RedistributeStats(common, Config.PConfig.Instance.TamedNpcDistribution.Value, Config.PConfig.Instance.ExtraStrongTamedNpc.Value);
					break;

				case NpcKind.Newborn:
					{
						DistributionMode mode = Config.PConfig.Instance.EnemiesDistribution.Value;
						if (mode == DistributionMode.Keep)
						{
							PLogger.LogError($"RedistributeStats: {mode} is not supported for newborns. Swithcing to random");
							mode = DistributionMode.Random;
						}

						StatDistributor.RedistributeStats(common, mode, Config.PConfig.Instance.ExtraStrongNewborns.Value);
					}
					break;
				default:
					break;
			}
		}
	}
}

using HarmonyLib;
using YotanModCore;
using YotanModCore.Consts;

namespace NpcStats.Patches
{
	public class NpcSpawnPatch
	{
		[HarmonyPatch(typeof(NPCManager), nameof(NPCManager.NPCLevelSet))]
		[HarmonyPostfix]
		public static void Post_NPCLevelSet(CommonStates common)
		{
			for (int i = 1; i <= common.level; i++) {
				common.statusPoint += StatsUtils.CalculateStatsGain(i);
			}

			var statsData = StatsUtils.GetStatsData(common);
			common.maxLife = statsData.life + common.level * statsData.upLife;
			common.attack = statsData.attack + common.level * statsData.upAttack;
			common.maxFaint = statsData.faint + common.level * statsData.upFaint;
			common.life = common.maxLife;
			common.faint = common.maxFaint;

			bool canIncrease = true;
			while (canIncrease) {
				int healthCost = StatsUtils.GetStatsUpCost(common, Stat.Health);
				int atkCost = StatsUtils.GetStatsUpCost(common, Stat.Attack);
				int speedCost = StatsUtils.GetStatsUpCost(common, Stat.Agility);

				long points = common.statusPoint;
				canIncrease = healthCost <= points || atkCost <= points || speedCost <= points;
				if (canIncrease) {
					int stats = UnityEngine.Random.Range(0, 300) % 3;
					StatsUtils.StatsUp(common, stats);
				}
			}
		}
	}
}
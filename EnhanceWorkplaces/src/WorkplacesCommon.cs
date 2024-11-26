using System;

namespace EnhanceWorkplaces
{
	internal class WorkplacesCommon
	{
		public static int GetLv1Quantity(CommonStates common)
		{
			int bonus = (int) (UnityEngine.Random.Range(0, (common.level / 10 + common.moral / 25f) * 1000f) / 1000f);
			int final = Math.Clamp(1 + bonus, 1, 10);
			
			if (Config.Instance.LogBonus.Value)
				PLogger.LogInfo($"> WorkPlace Lv1 reward: {final} items (Bonus: {bonus})");
			
			return final;
		}

		public static int GetLv2Quantity(CommonStates common)
		{
			int bonus = (int) (UnityEngine.Random.Range(0, (common.level / 20 + common.moral / 50f) * 1000f) / 1000f);
			int final = Math.Clamp(1 + bonus, 1, 5);

			if (Config.Instance.LogBonus.Value)
				PLogger.LogInfo($"> WorkPlace Lv2 reward: {final} items (Bonus: {bonus})");

			return final;
		}

		public static int GetLv3Quantity(CommonStates common)
		{
			int bonus = (int) (UnityEngine.Random.Range(0, (common.level / 30 + common.moral / 75f) * 1000f) / 1000f);
			int final = Math.Clamp(1 + bonus, 1, 3);

			if (Config.Instance.LogBonus.Value)
				PLogger.LogInfo($"> WorkPlace Lv3 reward: {final} items (Bonus: {bonus})");

			return final;
		}

		public static void OnWorkComplete(CommonStates common)
		{
			common.ExpChange(20f);
			common.water -= 1f;
		}
	}
}
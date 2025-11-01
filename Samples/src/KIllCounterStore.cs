using System;
using System.Xml.Serialization;
using HarmonyLib;
using YotanModCore;
using YotanModCore.DataStore;

namespace Samples
{
	public class KillCountStore : IGameDataStore
	{
		public class Data : ISaveData
		{
			public int killCount = 0;

			public Type GetStoreType()
			{
				return typeof(KillCountStore);
			}
		}

		public int killCount = 0;

		public void Initialize(GameManager gameManager)
		{
			PLogger.LogDebug("KillCountStore: Initialize");
		}

		public void OnLoad(ISaveData data)
		{
			killCount = (data as Data)?.killCount ?? 0;
			PLogger.LogInfo($"Loaded Kill Count: {killCount}");
		}

		public ISaveData OnSave()
		{
			PLogger.LogInfo($"Saving Kill Count: {killCount}");
			return new Data { killCount = killCount };
		}
	}

	public class KillCountPatch
	{
		[HarmonyPatch(typeof(CommonStates), nameof(CommonStates.NPCDeath))]
		[HarmonyPrefix]
		private static void Pre_CommonStates_NPCDeath()
		{
			PLogger.LogInfo("NPC Death");
			Managers.mn.gameMN.GetData<KillCountStore>().killCount++;
		}
	}
}

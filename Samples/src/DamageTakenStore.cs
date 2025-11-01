using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using HarmonyLib;
using UnityEngine;
using YotanModCore.DataStore;

namespace Samples
{
	public class DamageTakenStore : ICommonSDataStore
	{
		public class Data : ISaveData
		{
			public float DamageTaken;

			public Type GetStoreType()
			{
				return typeof(DamageTakenStore); // This MUST be the type of the DataStore class.
			}
		}

		private float _damageTaken;

		public float DamageTaken {
			get => _damageTaken;
			// Avoid overflow
			set => _damageTaken = Math.Clamp(value, 0, 10000f);
		}

		public void Initialize(CommonStates commonStates)
		{
			/* We can do some initialization here */
		}

		public void OnLoad(ISaveData data)
		{
			// Copy values from Data to DataStore
			this.DamageTaken = ((Data)data).DamageTaken;
		}

		public ISaveData OnSave()
		{
			// Copy values from DataStore to Data
			return new Data { DamageTaken = DamageTaken };
		}

		public bool IsBattleHardened()
		{
			return DamageTaken > 100;
		}
	}

	public class DamageTakenPatch
	{
		[HarmonyPatch(typeof(CommonStates), "NPCDamage")]
		[HarmonyPostfix]
		public static void Post_DamageTaken_Update(CommonStates __instance, float damageRate)
		{
			if (__instance == null)
				return;

			var store = __instance.GetData<DamageTakenStore>();
			store.DamageTaken += damageRate;
		}
	}
}

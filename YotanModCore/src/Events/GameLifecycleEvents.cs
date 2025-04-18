using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace YotanModCore.Events
{
	public class GameLifecycleEvents
	{
		public delegate void OnGameStart();

		/// <summary>
		/// Fired once a play session finishes loading in the game world.
		/// </summary>
		public static event OnGameStart OnGameStartEvent;

		public delegate void OnGameEnd();

		/// <summary>
		/// Fired whenever the player leaves the game world.
		/// </summary>
		public static event OnGameEnd OnGameEndEvent;

		private static IEnumerator GameStartRoutine()
		{
			yield return new WaitUntil(() => !SaveManager.SaveStatic.loaded);
			OnGameStartEvent?.Invoke();
		}

		[HarmonyPatch(typeof(GameManager), "Start")]
		[HarmonyPrefix]
		private static void Pre_GamaManager_Start(GameManager __instance)
		{
			if (!__instance.IsGameScene())
				return;

			__instance.StartCoroutine(GameStartRoutine());
		}

		[HarmonyPatch(typeof(SceneScript), "SceneChange")]
		[HarmonyPrefix]
		private static void Pre_SceneScript_SceneChange()
		{
			if (!Managers.mn.gameMN.IsGameScene())
				return;

			OnGameEndEvent?.Invoke();
		}
	}
}
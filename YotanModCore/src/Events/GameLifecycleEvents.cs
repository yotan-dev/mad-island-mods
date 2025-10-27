using System.Collections;
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

		internal static IEnumerator GameStartHandler(GameManager __instance)
		{
			if (!__instance.IsGameScene())
				yield break;

			yield return new WaitUntil(() => !SaveManager.SaveStatic.loaded);
			OnGameStartEvent?.Invoke();
		}

		internal static void GameEndHandler()
		{
			if (!Managers.mn.gameMN.IsGameScene())
				return;

			OnGameEndEvent?.Invoke();
		}
	}
}

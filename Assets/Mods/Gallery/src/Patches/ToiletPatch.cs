using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using YotanModCore;
using Gallery.GalleryScenes;
using UnityEngine.UIElements.Collections;

namespace Gallery.Patches
{
	public class ToiletPatch
	{
		public struct ToiletInfo
		{
			public CommonStates User;
			public CommonStates Target;
			public InventorySlot.Type ToiletType;
			public int ToiletSize;

			public ToiletInfo(CommonStates user, CommonStates target, InventorySlot.Type toiletType, int toiletSize)
			{
				this.User = user;
				this.Target = target;
				this.ToiletType = toiletType;
				this.ToiletSize = toiletSize;
			}
		}

		public delegate void OnSceneInfo(ToiletInfo info);

		public static event OnSceneInfo OnStart;

		public static event OnSceneInfo OnEnd;

		private static Dictionary<string, CommonStates> GetToiletCharas(SexManager manager, ToiletState state)
		{
			CommonStates player = Managers.mn.gameMN.playerCommons[GameManager.selectPlayer];
			CommonStates target;
			if (state == 0) {
				target = Managers.mn.inventory.itemSlot[50].common;
			} else {
				target = manager.tmpCommonGirl;
			}

			return new Dictionary<string, CommonStates>() {
				{ "user", player },
				{ "target", target },
			};
		}

		private static Dictionary<string, string> GetToiletInfos(ToiletState state, InventorySlot tmpToile)
		{
			return new Dictionary<string, string>() {
				{ "state", $"{state} ({(int)state})" },
				{ "tmpToile", $"{tmpToile?.name ?? "null"}, type: {tmpToile?.type.ToString() ?? "null"}, size: {tmpToile?.size.ToString() ?? "null"}" },
			};
		}

		[HarmonyPatch(typeof(SexManager), "Toilet")]
		[HarmonyPrefix]
		private static void Pre_SexManager_Toilet(SexManager __instance, int state, InventorySlot tmpToile = null)
		{
			if (Plugin.InGallery)
				return;

			ToiletState state_ = (ToiletState)state;
			try {
				var chars = GetToiletCharas(__instance, state_);
				GalleryLogger.SceneStart("Toilet", chars, GetToiletInfos(state_, tmpToile), false);

				switch (state_) {
				case ToiletState.Start:
					OnStart?.Invoke(new ToiletInfo(chars["user"], chars["target"], tmpToile?.type ?? InventorySlot.Type.None, tmpToile?.size ?? -1));
					break;

				case ToiletState.Insert:
				case ToiletState.StartPissHold:
				case ToiletState.Leave:
				case ToiletState.InsertedHoldMove:
				case ToiletState.Speed:
				case ToiletState.Bust:
				case ToiletState.ShowFace:
					/* Nothing */
					break;

				default:
					GalleryLogger.LogError($"Pre_SexManager_Toilet: Unknown state: {state_}");
					break;
				}
			
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("Toilet", error);
			}
		}

		[HarmonyPatch(typeof(SexManager), "Toilet")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_Toilet(IEnumerator result, SexManager __instance, int state, InventorySlot tmpToile = null)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			ToiletState state_ = (ToiletState)state;
			try {
				var chars = GetToiletCharas(__instance, state_);
				GalleryLogger.SceneEnd("Toilet", chars, GetToiletInfos(state_, tmpToile), false);

				switch (state_) {
				case ToiletState.Start:
					OnEnd?.Invoke(new ToiletInfo(chars["user"], chars["target"], tmpToile?.type ?? InventorySlot.Type.None, tmpToile?.size ?? -1));
					break;

				case ToiletState.Insert:
				case ToiletState.StartPissHold:
				case ToiletState.Leave:
				case ToiletState.InsertedHoldMove:
				case ToiletState.Speed:
				case ToiletState.Bust:
				case ToiletState.ShowFace:
					/* Nothing */
					break;

				default:
					GalleryLogger.LogError($"Post_SexManager_Toilet: Unknown state: {state_}");
					break;
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("Toilet", error);
			}
		}
	}
}
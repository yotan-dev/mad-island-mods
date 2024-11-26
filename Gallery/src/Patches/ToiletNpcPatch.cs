using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using YotanModCore;
using Gallery.GalleryScenes;
using UnityEngine.UIElements.Collections;

namespace Gallery.Patches
{
	public class ToiletNpcPatch
	{
		public struct ToiletNpcInfo
		{
			public CommonStates User;
			public CommonStates Target;
			public SexPlace SexPlace;

			public ToiletNpcInfo(CommonStates user, CommonStates target, SexPlace sexPlace)
			{
				this.User = user;
				this.Target = target;
				this.SexPlace = sexPlace;
			}
		}

		public delegate void OnSceneInfo(ToiletNpcInfo info);

		public static event OnSceneInfo OnStart;

		public static event OnSceneInfo OnEnd;

		private static Dictionary<string, CommonStates> GetNPCToiletCharas(CommonStates npcA, SexPlace sexPlace)
		{
			InventorySlot tmpInventory = sexPlace.GetComponent<InventorySlot>();
			ItemSlot tmpSlot = tmpInventory.slots[0];

			return new Dictionary<string, CommonStates>() {
				{ "user", npcA },
				{ "target", tmpSlot.common },
			};
		}

		private static Dictionary<string, string> GetNPCToiletInfos(SexPlace sexPlace)
		{
			return new Dictionary<string, string>() {
				{ "sexPlace", $"{sexPlace?.name ?? "*null*"}, grade: {sexPlace?.grade ?? -1} type: {sexPlace?.placeType.ToString() ?? "*null*"}" },
			};
		}

		[HarmonyPatch(typeof(SexManager), "ToiletNPC")]
		[HarmonyPrefix]
		private static void Pre_SexManager_ToiletNPC(CommonStates npcA, SexPlace sexPlace)
		{
			if (Plugin.InGallery)
				return;

			try
			{
				var chars = GetNPCToiletCharas(npcA, sexPlace);

				GalleryLogger.SceneStart("ToiletNPC", GetNPCToiletCharas(npcA, sexPlace), GetNPCToiletInfos(sexPlace));

				OnStart?.Invoke(new ToiletNpcInfo(chars["user"], chars["target"], sexPlace));
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("ToiletNPC", error);
			}
		}

		[HarmonyPatch(typeof(SexManager), "ToiletNPC")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_ToiletNPC(IEnumerator result, CommonStates npcA, SexPlace sexPlace)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try
			{
				var chars = GetNPCToiletCharas(npcA, sexPlace);

				GalleryLogger.SceneEnd("ToiletNPC", chars, GetNPCToiletInfos(sexPlace));
				OnEnd?.Invoke(new ToiletNpcInfo(chars["user"], chars["target"], sexPlace));
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("ToiletNPC", error);
			}
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using YotanModCore;
using Gallery.GalleryScenes;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;
using UnityEngine.UIElements.Collections;
using YotanModCore.Consts;

namespace Gallery.Patches
{
	public class StoryPatches
	{
		private static bool MainTakumi00Progress = false;
		private static bool MainTakumi01Progress = false;
		
		private static bool PrisonProgress = false;

		private static bool GiantProgress = false;

		private static void TryUnlock(string description, GalleryChara charA, GalleryChara charB, StoryFlag flag)
		{
				var interaction = new StoryInteraction(
					charA,
					charB,
					flag
				);
				
				if (GalleryState.Instance.Story.Add(interaction)) {
					GalleryLogger.LogDebug($"StoryManager: {description} event UNLOCKED");
				} else {
					GalleryLogger.LogDebug($"StoryManager: {description} event was already unlocked");
				}
		}

		[HarmonyPatch(typeof(StoryManager), "BossCyborg00")]
		[HarmonyPostfix]
		private static IEnumerator Post_StoryManager_BossCyborg00(IEnumerator result, StoryManager __instance)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			GalleryLogger.LogDebug($"StoryManager: BossCyborg00: {__instance.bossBattleState}");
			if ((BossBattleState) __instance.bossBattleState == BossBattleState.PlayerDefeated) {
				GalleryState.Instance.Story.Add(
					new StoryInteraction(
						new GalleryChara(Managers.mn?.gameMN?.playerCommons?[GameManager.selectPlayer]),
						new GalleryChara(NpcID.Cassie),
						StoryFlag.PlayerDefeatedByCyborg
					)
				);

				GalleryLogger.LogDebug("StoryManager: BossCyborg00: PlayerDefeated event UNLOCKED");
			} else if ((BossBattleState) __instance.bossBattleState == BossBattleState.BossDefeated) {
				GalleryState.Instance.Story.Add(
					new StoryInteraction(
						new GalleryChara(Managers.mn?.gameMN?.playerCommons?[GameManager.selectPlayer]),
						new GalleryChara(NpcID.Cassie),
						StoryFlag.PlayerDefeatsCyborg
					)
				);

				GalleryLogger.LogDebug("StoryManager: BossCyborg00: BossDefeated event UNLOCKED");
			} else {
				GalleryLogger.LogDebug($"StoryManager: BossCyborg00: No event to unlock ({__instance.bossBattleState})");
			}
		}

		[HarmonyPatch(typeof(StoryManager), "Anim")]
		[HarmonyPrefix]
		private static void Pre_StoryManager_Anim(string animName)
		{
			if (MainTakumi00Progress && animName == "A_down_raped" && !Plugin.InGallery) {
				TryUnlock(
					"MainTakumi000: Reika",
					new GalleryChara(NpcID.Takumi),
					new GalleryChara(NpcID.Reika),
					StoryFlag.TakumiDrugRapesReika
				);
			} else if (PrisonProgress && animName == "Rapes_A_Start_Loop_01" && !Plugin.InGallery) {
				TryUnlock(
					"Prison: Punish sally w/ Takumi",
					new GalleryChara(NpcID.Man),
					new GalleryChara(NpcID.Sally),
					StoryFlag.Man3PSally
				);
			} else if (MainTakumi01Progress && animName == "B_dogezaBackToDown" && !Plugin.InGallery) {
				TryUnlock(
					"MainTakumi01: Yona",
					new GalleryChara(NpcID.Takumi),
					new GalleryChara(NpcID.Yona),
					StoryFlag.TakumiRapeYona
				);
			} else if (GiantProgress && animName == "Rape_A_Loop_02" && !Plugin.InGallery) {
				TryUnlock(
					"Sub_Giant: Rape",
					new GalleryChara(NpcID.Man),
					new GalleryChara(NpcID.Giant),
					StoryFlag.ManRapeGiant
				);
			}
		}

		[HarmonyPatch(typeof(StoryManager), "MainTakumi00")]
		[HarmonyPostfix]
		private static IEnumerator Post_StoryManager_MainTakumi00(IEnumerator result)
		{
			MainTakumi00Progress = true;

			while (result.MoveNext())
				yield return result.Current;

			MainTakumi00Progress = false;
		}

		[HarmonyPatch(typeof(StoryManager), "MainTakumi01")]
		[HarmonyPostfix]
		private static IEnumerator Post_StoryManager_MainTakumi01(IEnumerator result)
		{
			MainTakumi01Progress = true;

			while (result.MoveNext())
				yield return result.Current;

			MainTakumi01Progress = false;
		}

		[HarmonyPatch(typeof(StoryManager), "MainReika00")]
		[HarmonyPostfix]
		private static IEnumerator Post_StoryManager_MainReika00(IEnumerator result, StoryManager __instance)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			if (__instance.commandState == 0) {
				TryUnlock(
					"MainReika00: KeigoReikaBj",
					new GalleryChara(NpcID.Keigo),
					new GalleryChara(NpcID.Reika),
					StoryFlag.KeigoReikaBj
				);
			} else {
				GalleryLogger.LogDebug("StoryManager: MainReika00: Scream doesn't unlokck it");
			}
		}
	
		[HarmonyPatch(typeof(StoryManager), "ProgressChange")]
		[HarmonyPostfix]
		private static void Post_StoryManager_ProgressChange(StoryManager __instance, string questKey, int progress)
		{
			switch (questKey) {
			case "Main_Reika":
				switch (progress) {
				case 2: // (Keigo01 / observed -- Reika riding Keigo in the bushes)
					TryUnlock(
						"Keigo01 / observed",
						new GalleryChara(NpcID.Keigo),
						new GalleryChara(NpcID.Reika),
						StoryFlag.KeigoReikaCowgirl
					);
					break;

				case 3: // (Keigo01 / screamed -- nothing happens)
					/* nothing to do here */
					break;
				}
				break;

			case "Main_Takumi1":
				switch (progress) {
				case 1: // MainTakumi01 / kill
				case 2: // Tie up
					/* nothing to do here */
					break;
				}
				break;

			case "Sub_Giant":
				switch (progress) {
				case 1: // Leave / uninsteristing
				case 2: // kill
					/* nothing to do here */
					break;

				case 3: // rape + tie up
					break;

				case 4: // Interesting / friendly fuck
					TryUnlock(
						"Sub_Giant: sex",
						new GalleryChara(NpcID.Man),
						new GalleryChara(NpcID.Giant),
						StoryFlag.ManSexGiant
					);
					break;
				}
				break;

			case "Sub_Prison":
				switch (progress) {
				case 3: // Dildo
					TryUnlock(
						"Prison01 / Dildo",
						new GalleryChara(NpcID.Man),
						new GalleryChara(NpcID.Sally),
						StoryFlag.SallyDildo
					);
					break;
				case 4: // Fleshlight
					TryUnlock(
						"Prison01 / Fleshlight",
						new GalleryChara(NpcID.Man),
						new GalleryChara(NpcID.Sally),
						StoryFlag.SallyFleshlight
					);
					break;

				case 2: // Didn't punish / didn't fuck
				case 5: // Kill Sally
				case 6: // Tie up sally
					/* nothing to do here */
					break;
				}
				break;
			}
			// this.ProgressChange("Main_Reika", 2);
		}

		[HarmonyPatch(typeof(StoryManager), "MainReika01")]
		[HarmonyPrefix]
		private static void Post_StoryManager_MainReika01(StoryManager __instance)
		{
			GalleryLogger.LogDebug(">>>>>> MainReika001");
		}

		[HarmonyPatch(typeof(StoryManager), "Keigo00")]
		[HarmonyPrefix]
		private static void Post_StoryManager_Keigo00(StoryManager __instance)
		{
			GalleryLogger.LogDebug(">>>>>> Keigo00");
		}

		// [HarmonyPatch(typeof(StoryManager), "Shino00")]
		// [HarmonyPrefix]
		// private static void Post_StoryManager_Shino00(StoryManager __instance)
		// {
		// 	// All interactions with shino
		// 	// @FIXME: Shino name is in japanese if you insult her
		// 	GalleryLogger.LogDebug(">>>>>> Shino00");
		// }

		[HarmonyPatch(typeof(StoryManager), "Prison")]
		[HarmonyPostfix]
		private static IEnumerator Post_StoryManager_Prison(IEnumerator result, StoryManager __instance)
		{
			// Paths 
			// Sally -> Punish (Scene 1) -- Checked by Anim
			// Sally -> Punish -> Tie (Slave) -- Handled by progress change
			// Sally -> Leave -> Do -- Handled by progress change
			PrisonProgress = true;

			while (result.MoveNext())
				yield return result.Current;

			PrisonProgress = false;

			if (Plugin.InGallery)
				yield break;
		}

		[HarmonyPatch(typeof(StoryManager), "Daruman01")]
		[HarmonyPostfix]
		private static IEnumerator Post_StoryManager_Daruman01(IEnumerator result, StoryManager __instance)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			if ((BossBattleState) __instance.bossBattleState == BossBattleState.PlayerDefeated && GameManager.selectPlayer == 0) {
				GalleryState.Instance.Story.Add(
					new StoryInteraction(
						new GalleryChara(NpcID.Yona),
						new GalleryChara(NpcID.Dalman),
						StoryFlag.PlayerDefeatedByDaruman
					)
				);

				GalleryLogger.LogDebug("StoryManager: Daruma01: PlayerDefeated event UNLOCKED");
			}
		}

		[HarmonyPatch(typeof(StoryManager), "BossHunter00")]
		[HarmonyPostfix]
		private static IEnumerator Post_StoryManager_BossHunter00(IEnumerator result, StoryManager __instance)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			if ((BossBattleState) __instance.bossBattleState == BossBattleState.PlayerDefeated && GameManager.selectPlayer == 0) {
				GalleryState.Instance.Story.Add(
					new StoryInteraction(
						new GalleryChara(NpcID.Yona),
						new GalleryChara(NpcID.Bandana),
						StoryFlag.PlayerDefeatedByHunters
					)
				);

				GalleryLogger.LogDebug("StoryManager: BossHunter00: PlayerDefeated event UNLOCKED");
			}
		}

		[HarmonyPatch(typeof(StoryManager), "SubGiant00")]
		[HarmonyPostfix]
		private static IEnumerator Post_StoryManager_SubGiant00(IEnumerator result, StoryManager __instance)
		{
			GiantProgress = true;

			while (result.MoveNext())
				yield return result.Current;

			GiantProgress = false;

			if (Plugin.InGallery)
				yield break;
		}
	}
}
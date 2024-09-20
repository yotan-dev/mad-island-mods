using System;
using System.Collections;
using HarmonyLib;
using TMPro;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace YoUnnoficialPatches.Patches
{
	/**
	 * Translates some game UI from Japanese to English
	 */
	public static class TranslationPatch
	{
		private static void TryFixText(GameObject baseObj, string textPath, string newText) {
			try {
				if (baseObj == null) {
					throw new Exception("baseObj is null");
				}

				baseObj.transform.Find(textPath).GetComponent<TextMeshProUGUI>().text = newText;
			} catch (Exception ex) {
				PLogger.LogError($"Failed to fix text {textPath} ({newText}). Error: {ex}");
			}
		}

		[HarmonyPatch(typeof(StoryManager), "BossBattleCheck")]
		[HarmonyPrefix]
		private static void Pre_StoryManager_BossBattleCheck(CommonStates bossCommon, bool teleport)
		{
			switch (bossCommon?.npcID) {
			case NpcID.Cassie:	bossCommon.charaName = "Cassie"; break;
			case NpcID.Takumi:	bossCommon.charaName = "Takumi"; break;
			case NpcID.Scythe:	bossCommon.charaName = "Scythe"; break;
			case NpcID.Planton:	bossCommon.charaName = "Planton"; break; // @TODO: Other monsters
			case NpcID.Sally:	bossCommon.charaName = "Sally"; break;
			}
		}

		private static IEnumerator FixUITexts()
		{
			yield return new WaitUntil(() => Managers.uiManager != null);
			
			// Craft/Shop "Ingredients"
			TryFixText(Managers.uiManager.needPanel, "NeedText", "Requirements");
		
			// Shop title
			TryFixText(Managers.uiManager.UIManager?.shopPanel, "InventoryName (1)/Text (TMP)", "Buy");

			// Paused text
			TryFixText(GameObject.Find("StaticGroup/FrontCanvas"), "PauseText", "Paused... <size=10>(Esc to resume)</size>");

			// Surgery table / Beauty mirror (Edit)
			TryFixText(Managers.uiManager.UIManager?.editPanel, "Text (1)", "Shape");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ShapePanel/Hair/Text", "Forelock");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ShapePanel/BackHair/Text", "Hair");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ShapePanel/Face/Text", "Eyes");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ShapePanel/Nose/Text", "Nose");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ShapePanel/EyeBrow/Text", "Eyebrow");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ShapePanel/HighLight/Text", "Eye 2");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ShapePanel/Body/Text", "Chest");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ShapePanel/Tatoo/Text", "Paint");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ShapePanel/Ear/Text", "Ear");

			TryFixText(Managers.uiManager.UIManager?.editPanel, "Text (2)", "Color");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ColorPanel/Hair/Text", "Forelock");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ColorPanel/HairBack/Text", "Back hair");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ColorPanel/EyeBrow/Text", "Eyebrows");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ColorPanel/Skin/Text", "Skin");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ColorPanel/Eye/Text", "Pupil");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ColorPanel/EyeWhite/Text", "Eyes BG");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "ColorPanel/Nipple/Text", "Nipples");
			
			TryFixText(Managers.uiManager.UIManager?.editPanel, "CancelButton/Text", "Cancel");
			TryFixText(Managers.uiManager.UIManager?.editPanel, "OKButton/Text", "Change");
		}

		[HarmonyPatch(typeof(UIManager), "Awake")]
		[HarmonyPostfix]
		private static void Post_UIManager_Awake(UIManager __instance)
		{
			__instance.StartCoroutine(FixUITexts());
		}

		[HarmonyPatch(typeof(StoryManager), nameof(StoryManager.QuestNameSet))]
		[HarmonyPostfix]
		private static void Post_StoryManager_QuestNameSet()
		{
			// Quest log
			TryFixText(Managers.uiManager.UIManager?.questDiaryPanel, "yonaNameBack/yonaNameText", "Yona");
			TryFixText(Managers.uiManager.UIManager?.questDiaryPanel, "commonBack/commonText", "Common");
			TryFixText(Managers.uiManager.UIManager?.questDiaryPanel, "manNameBack/manNameText", "Man");
		}
	}
}
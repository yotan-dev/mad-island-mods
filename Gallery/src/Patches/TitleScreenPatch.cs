using HarmonyLib;
using YotanModCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gallery.Patches
{
	public class TitleScreenPatch
	{
		[HarmonyPatch(typeof(TitleScript), "Start")]
		[HarmonyPrefix]
		private static void Pre_TitleScript_Start()
		{
			Managers.mn.sexMN.gallery = false;
			Plugin.InGallery = false;
			
			var galleryBtn = GameObject.Find("TitleCanvas/TitleMenuPanel/GalleryButton");
			if (galleryBtn != null)
			{
				return;
			}

			var titleMenuPanel = GameObject.Find("TitleCanvas/TitleMenuPanel");
			var galleryBtnPrefab = GameObject.Instantiate(Plugin.Assets.LoadAsset<GameObject>("GalleryButton"), titleMenuPanel.transform);
			galleryBtnPrefab.transform.SetAsFirstSibling();
		}


		[HarmonyPatch(typeof(StoryManager), "Awake")]
		[HarmonyPrefix]
		private static void Pre_StoryManager_Awake(StoryManager __instance, ref bool __runOriginal)
		{
			if (!Plugin.InGallery)
			{
				return;
			}

			__runOriginal = false;
			
			StoryManager.demo = __instance.demoCheck;
			__instance.questMN = __instance.GetComponent<QuestManager>();

			GameObject.Instantiate(Plugin.Assets.LoadAsset<GameObject>("GalleryMngr"), GameObject.Find("StaticGroup").transform);
			
			// __instance.mn = GameObject.Find("Managers").GetComponent<ManagersScript>();
		}
	}
}
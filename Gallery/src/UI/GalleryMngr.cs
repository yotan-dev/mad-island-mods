using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using YotanModCore;
using Gallery.GalleryScenes;
using Gallery.SaveFile;
using Gallery.UI.SceneController;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using YotanModCore.Consts;
using ExtendedHSystem.Scenes;

namespace Gallery.UI
{
	public class GalleryMngr : MonoBehaviour
	{
		private ManagersScript mn;

		[HideInInspector]
		public NPCManager NpcManager;

		public RectTransform ScenesListArea;

		public GameObject GallerySceneGroupPrefab;

		public GameObject GallerySceneBtnPrefab;

		public GameObject DummySexMeter;

		private IScene ActiveScene;

		private static Func<GallerySceneInfo.SceneNpc, GallerySceneInfo.SceneNpc, bool> PlayerRapedCheck = (GallerySceneInfo.SceneNpc npcA, GallerySceneInfo.SceneNpc npcB) =>
			GalleryState.Instance.PlayerRaped.Any((x) =>
				x.Character1.UnlockCheck(npcA)
				&& x.Character2.UnlockCheck(npcB)
			);

		private static Func<GallerySceneInfo.SceneNpc, GallerySceneInfo.SceneNpc, bool> ManRapesCheck = (GallerySceneInfo.SceneNpc npcA, GallerySceneInfo.SceneNpc npcB) =>
			GalleryState.Instance.ManRapes.Any((x) => (
				x.Character1.UnlockCheck(npcA)
				&& x.Character2.UnlockCheck(npcB)
			));

		private static Func<GallerySceneInfo.SceneNpc, GallerySceneInfo.SceneNpc, bool> ManSleepRapesCheckNormal = (GallerySceneInfo.SceneNpc npcA, GallerySceneInfo.SceneNpc npcB) =>
			GalleryState.Instance.SleepRapes.Any((x) => (
				x.Character1.UnlockCheck(npcA)
				&& x.Character2.UnlockCheck(npcB)
				&& x.SexType == ManRapesSexType.Rape
			));

		private static Func<GallerySceneInfo.SceneNpc, GallerySceneInfo.SceneNpc, bool> ManSleepRapesCheckDiscretly = (GallerySceneInfo.SceneNpc npcA, GallerySceneInfo.SceneNpc npcB) =>
			GalleryState.Instance.SleepRapes.Any((x) => (
				x.Character1.UnlockCheck(npcA)
				&& x.Character2.UnlockCheck(npcB)
				&& x.SexType == ManRapesSexType.DiscretlyRape
			));

		private static Func<GallerySceneInfo.SceneNpc, GallerySceneInfo.SceneNpc, bool> CommonSexNpcCheck = (GallerySceneInfo.SceneNpc npcA, GallerySceneInfo.SceneNpc npcB) =>
			GalleryState.Instance.CommonSexNpc.Any((x) => (
				x.Character1.UnlockCheck(npcA)
				&& x.Character2.UnlockCheck(npcB)
			));

		// private List<GallerySceneInfo> Scenes = new List<GallerySceneInfo>() {
		//#region AssWall
		/*
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.Man,
			SceneType = GallerySceneInfo.SceneTypes.AssWall,
			Name = "Ass Toilet\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Man, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.FemaleNative, Pregnant = false },
			Prop = "toilet_01", // @TODO: Review
			IsUnlocked = true, // (GallerySceneInfo.SceneNpc npcA, GallerySceneInfo.SceneNpc npcB) =>
				GalleryState.Instance.AssWall.Any((x) => (
					x.Character1.UnlockCheck(npcA)
					&& x.Character2.UnlockCheck(npcB)
					&& x.WallType.Equals("Toilet")
				)),
		},
		*/
		//#endregion
		//#region PlayerRaped
		/*
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.Yona,
			SceneType = GallerySceneInfo.SceneTypes.PlayerRaped,
			Name = "Defeated by\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Yona, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.MaleNative, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.PlayerRapedCheck,
		},
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.Yona,
			SceneType = GallerySceneInfo.SceneTypes.PlayerRaped,
			Name = "Defeated by\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Yona, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.BigMaleNative, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.PlayerRapedCheck
		},
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.Yona,
			SceneType = GallerySceneInfo.SceneTypes.PlayerRaped,
			Name = "Defeated by\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Yona, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Werewolf, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.PlayerRapedCheck,
		},
		//#endregion
		//#region ManRapes
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.Man,
			SceneType = GallerySceneInfo.SceneTypes.ManRapes,
			Name = "{npcA} rapes\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Man, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.FemaleNative, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.ManRapesCheck,
		},
		new GallerySceneInfo() {
			RequireDLC = true,
			CharGroup = GallerySceneInfo.CharGroups.Man,
			SceneType = GallerySceneInfo.SceneTypes.ManRapes,
			Name = "{npcA} rapes\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Man, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.NativeGirl, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.ManRapesCheck
		},
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.Man,
			SceneType = GallerySceneInfo.SceneTypes.ManRapes,
			Name = "{npcA} rapes\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Man, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.UnderGroundWoman, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.ManRapesCheck,
		},
		//#endregion

		//#region Sleep Rapes
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.Man,
			SceneType = GallerySceneInfo.SceneTypes.SleepRaes,
			Name = "{npcA} sleep rapes\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Man, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.FemaleNative, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.ManSleepRapesCheckNormal,
		},
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.Man,
			SceneType = GallerySceneInfo.SceneTypes.SleepRaes,
			Name = "{npcA} sleep rapes\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Man, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.NativeGirl, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.ManSleepRapesCheckNormal,
		},
		//#endregion

		//#region Common Sex Player
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.Man,
			SceneType = GallerySceneInfo.SceneTypes.CommonSexPlayer,
			Name = "{npcA} fucks\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.Man, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.FemaleNative, Pregnant = false },
			IsUnlocked = true, // (GallerySceneInfo.SceneNpc a, GallerySceneInfo.SceneNpc b) => true,
		},
		//#endregion
		//#region CommonSexNpc
		// @TODO: Check for SexType / PlaceType / PlaceGrade

		// ========== Native Female
		/*
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.NativeFemale,
			SceneType = GallerySceneInfo.SceneTypes.CommonSexNpc,
			Name = "{npcA} sex with\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.FemaleNative, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.MaleNative, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.CommonSexNpcCheck,
			Prop = "leafbed",
		},

		// ========== Native Male
		new GallerySceneInfo() {
			CharGroup = GallerySceneInfo.CharGroups.NativeMale,
			SceneType = GallerySceneInfo.SceneTypes.CommonSexNpc,
			Name = "{npcA} sex with\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.MaleNative, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.FemaleNative, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.CommonSexNpcCheck,
			Prop = "leafbed",
		},
		new GallerySceneInfo() {
			RequireDLC = true,
			CharGroup = GallerySceneInfo.CharGroups.NativeMale,
			SceneType = GallerySceneInfo.SceneTypes.CommonSexNpc,
			Name = "{npcA} sex with\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.MaleNative, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.NativeGirl, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.CommonSexNpcCheck,
			Prop = "leafbed",
		},
		new GallerySceneInfo() {
			RequireDLC = true,
			CharGroup = GallerySceneInfo.CharGroups.NativeMale,
			SceneType = GallerySceneInfo.SceneTypes.CommonSexNpc,
			Name = "{npcA} sex with\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.MaleNative, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.NativeGirl, Pregnant = true },
			IsUnlocked = true, // GalleryMngr.CommonSexNpcCheck,
			Prop = "leafbed",
		},

		// ========== Native Girl
		new GallerySceneInfo() {
			RequireDLC = true,
			CharGroup = GallerySceneInfo.CharGroups.NativeGirl,
			SceneType = GallerySceneInfo.SceneTypes.CommonSexNpc,
			Name = "{npcA} sex with\n{npcB}",
			NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.NativeGirl, Pregnant = false },
			NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcIDs.MaleNative, Pregnant = false },
			IsUnlocked = true, // GalleryMngr.CommonSexNpcCheck,
			Prop = "leafbed",
		},
		*/
		//#endregion

		// @TODO: SleepRapes , CommonSexPlayer, Toilet , ToiletNpc, Delivery , Story
		// };

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(1f); // Give it some time to start other stuff.. not sure why, but it is needed
			this.mn = GameObject.Find("Managers").GetComponent<ManagersScript>();
			this.mn.uiMN.MainCanvasView(false);
			GameObject.Find("GalleryCanvas").SetActive(false); // remove original gallery canvas, we have our own
			this.NpcManager = GameObject.Find("NPCManager").GetComponent<NPCManager>();

			this.mn.sexMN.sexMeter = this.DummySexMeter.GetComponent<Image>();

			Dictionary<string, GameObject> groups = new Dictionary<string, GameObject>();

			foreach (var scnManager in GalleryScenesManager.Instance.SceneManagers)
			{
				foreach (var scn in scnManager.GetScenes())
				{
					if (scn.RequireDLC && !GameInfo.HasDLC)
						continue;

					GameObject grp;
					if (!groups.TryGetValue(scn.CharGroup.ToString(), out grp))
					{
						grp = GameObject.Instantiate(this.GallerySceneGroupPrefab, this.ScenesListArea);
						groups[scn.CharGroup.ToString()] = grp;
					}

					var options = grp.transform.Find("Options").gameObject;
					var btnObject = GameObject.Instantiate(this.GallerySceneBtnPrefab, options.transform);
					var btn = btnObject.GetComponent<Button>();

					if (scn.IsUnlocked)
					{
						grp.transform.Find("Title").GetComponent<Text>().text = scn.CharGroup.ToString();

						btn.onClick.AddListener(() =>
						{
							base.StartCoroutine(this.DoScene(scn));
						});
						btn.enabled = true;
						btnObject.GetComponentInChildren<Text>().text = scn.Name
							.Replace("{npcA}", CommonUtils.GetName(scn.NpcA.NpcID))
							.Replace("{npcB}", CommonUtils.GetName(scn.NpcB.NpcID));
					}
					else
					{
						btn.enabled = false;
						btnObject.GetComponentInChildren<Text>().text = "????";
					}
				}
			}

			yield break;
		}

		private bool HasRandomGen(int npcID)
		{
			switch (npcID)
			{
				case NpcID.MaleNative:
				case NpcID.BigNative:
				case NpcID.FemaleNative:
				case NpcID.NativeGirl:
					return true;
				default:
					return false;
			}
		}


		private GameObject tmpNpc;

		private IEnumerator CreateNpc(GallerySceneInfo.SceneNpc npc)
		{
			var npcObj = GameObject.Instantiate(this.NpcManager.npcPrefab[(int)npc.NpcID], Managers.mn.sexMN.transform.position, Quaternion.identity);
			// DON't setActive(False) it or they will crash
			if (npc.NpcID == NpcID.Yona || npc.NpcID == NpcID.Man)
			{
				npcObj.GetComponent<PlayerMove>().enabled = false;
				npcObj.GetComponent<Rigidbody>().useGravity = false;
				yield return null;

				if (npc.NpcID == NpcID.Yona)
					yield return base.StartCoroutine(this.mn.randChar.SetPlayer(npcObj.GetComponent<CommonStates>()));

				Managers.mn.gameMN.playerCommons[GameManager.selectPlayer] = npcObj.GetComponent<CommonStates>();
			}
			else
			{
				yield return null;

				npcObj.GetComponent<NPCMove>().enabled = false;
				npcObj.GetComponent<CommonStates>().npcID = (int)npc.NpcID;

				if (this.HasRandomGen(npc.NpcID))
					this.mn.randChar.RandomGen(npcObj);
			}

			this.tmpNpc = npcObj;
		}


		private IEnumerator DoScene(GallerySceneInfo scene)
		{
			if (this.ActiveScene != null) {
				this.ActiveScene.Destroy();
				this.ActiveScene = null;
			}

			GallerySceneInfo.PlayData playData = new GallerySceneInfo.PlayData();

			yield return this.CreateNpc(scene.NpcA);
			var npcA = tmpNpc;
			yield return this.CreateNpc(scene.NpcB);
			var npcB = tmpNpc;

			playData.NpcA = npcA.GetComponent<CommonStates>();
			playData.NpcB = npcB.GetComponent<CommonStates>();

			if (scene.Prop != "")
			{
				ItemData tmpItem = this.mn.itemMN.FindItem(scene.Prop);
				if (tmpItem == null)
				{
					PLogger.LogError("Item not found: " + scene.Prop);
					yield break;
				}

				GameObject tmpBuild = GameObject.Instantiate<GameObject>(tmpItem.itemObj, Managers.mn.sexMN.transform.position, Quaternion.identity);

				playData.Prop = tmpBuild;

				// Give time for the prop to setup
				yield return null;
			}

			var playableScene = scene.GetScene(playData);
			this.ActiveScene = playableScene;
			yield return base.StartCoroutine(playableScene.Run());

			if (playData.Prop != null)
				GameObject.Destroy(playData.Prop);
			GameObject.Destroy(npcA);
			GameObject.Destroy(npcB);
		}

		public void BackToTitle()
		{
			Plugin.InGallery = false;
			this.mn.sceneSC.SceneChange("title_01");
		}
	}
}

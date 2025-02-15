using System.Collections;
using System.Collections.Generic;
using YotanModCore;
using Gallery.GalleryScenes;
using UnityEngine;
using UnityEngine.UI;
using YotanModCore.Consts;
using Gallery.ConfigFiles;
using UnityEngine.EventSystems;

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

		private BaseController ActiveController;

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(0.2f); // Give it some time to start other stuff.. I think this is needed so the Awake/Starts properly run
			this.mn = GameObject.Find("Managers").GetComponent<ManagersScript>();
			this.mn.uiMN.MainCanvasView(false);
			GameObject.Find("GalleryCanvas").SetActive(false); // remove original gallery canvas, we have our own
			this.NpcManager = GameObject.Find("NPCManager").GetComponent<NPCManager>();

			this.mn.sexMN.sexMeter = this.DummySexMeter.GetComponent<Image>();

			Dictionary<string, GameObject> groups = new Dictionary<string, GameObject>();

			GalleryScenesManager.Instance.LoadGallery();
			foreach (var scnGroup in GalleryScenesManager.Instance.SceneGroups)
			{
				foreach (var scn in scnGroup.Value.Scenes)
				{
					if (scn.RequiresDLC && !GameInfo.HasDLC)
						continue;

					GameObject grp;
					if (!groups.TryGetValue(scnGroup.Key, out grp))
					{
						grp = GameObject.Instantiate(this.GallerySceneGroupPrefab, this.ScenesListArea);
						groups[scnGroup.Key] = grp;
					}

					var options = grp.transform.Find("Options").gameObject;
					var btnObject = GameObject.Instantiate(this.GallerySceneBtnPrefab, options.transform);
					var btn = btnObject.GetComponent<Button>();

					if (scn.Controller.IsUnlocked(scn.Actors))
					{
						grp.transform.Find("Title").GetComponent<Text>().text = scnGroup.Key.ToString();

						btn.onClick.AddListener(() =>
						{
							base.StartCoroutine(this.DoScene(scn));
						});
						btn.enabled = true;
						var name = scn.Name;
						if (scn.Actors.Length > 0)
							name = name.Replace("[npc0]", CommonUtils.GetName(scn.Actors?[0]?.NpcId ?? -1));
						if (scn.Actors.Length > 1)
							name = name.Replace("[npc1]", CommonUtils.GetName(scn.Actors?[1]?.NpcId ?? -1));
						
						btnObject.GetComponentInChildren<Text>().text = name;
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

		private GameObject TmpNpc;

		private IEnumerator CreateNpc(GalleryActor actor)
		{
			var npcObj = GameObject.Instantiate(
				this.NpcManager.npcPrefab[(int)actor.NpcId],
				Managers.mn.sexMN.transform.position,
				Quaternion.identity
			);

			var common = npcObj.GetComponent<CommonStates>();
			if (actor.Pregnant)
			{
				common.pregnant[PregnantIndex.Father] = 1;
				common.pregnant[PregnantIndex.TimeToBirth] = 1;
			}

			if (actor.Fainted)
				common.faint = 0.0f;
			else
				common.faint = 1.0f;
			
			// DON't setActive(False) it or they will crash
			if (actor.NpcId == NpcID.Yona || actor.NpcId == NpcID.Man)
			{
				npcObj.GetComponent<PlayerMove>().enabled = false;
				npcObj.GetComponent<Rigidbody>().useGravity = false;
				yield return null;

				if (actor.NpcId == NpcID.Yona)
					yield return base.StartCoroutine(this.mn.randChar.SetPlayer(npcObj.GetComponent<CommonStates>()));

				// Managers.mn.gameMN.playerCommons[GameManager.selectPlayer] = npcObj.GetComponent<CommonStates>();
			}
			else
			{
				yield return null;

				common.npcID = (int)actor.NpcId;
				common.nMove.enabled = true;
				common.nMove.actType = NPCMove.ActType.Wait;
				
				if (this.HasRandomGen(actor.NpcId))
					this.mn.randChar.RandomGen(npcObj);
			}

			this.TmpNpc = npcObj;
		}

		private IEnumerator DoScene(GallerySceneConfig scene)
		{
			if (this.ActiveController != null) {
				this.ActiveController.Destroy();
				this.ActiveController = null;
			}

			EventSystem.current.SetSelectedGameObject(null);

			PlayData playData = new PlayData();
			
			foreach (var actor in scene.Actors)
			{
				yield return this.CreateNpc(actor);
				playData.Actors.Add(this.TmpNpc.GetComponent<CommonStates>());
			}
			
			if (scene.Controller.Prop != null && scene.Controller.Prop != "")
			{
				ItemData tmpItem = this.mn.itemMN.FindItem(scene.Controller.Prop);
				if (tmpItem == null)
				{
					PLogger.LogError("Item not found: " + scene.Controller.Prop);
					yield break;
				}

				GameObject tmpBuild = GameObject.Instantiate<GameObject>(tmpItem.itemObj, Managers.mn.sexMN.transform.position, Quaternion.identity);

				playData.Prop = tmpBuild;

				// Give time for the prop to setup
				yield return null;
			}

			this.ActiveController = scene.Controller;

			yield return base.StartCoroutine(scene.Controller.Play(playData));

			this.ActiveController = null;

			if (playData.Prop != null)
				GameObject.Destroy(playData.Prop);

			foreach (var actor in playData.Actors)
				GameObject.Destroy(actor.gameObject);
		}

		public void BackToTitle()
		{
			Plugin.InGallery = false;
			this.mn.sceneSC.SceneChange("title_01");
		}
	}
}

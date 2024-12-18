using System;
using System.Collections.Generic;
using ExtendedHSystem;
using Gallery.GalleryScenes;
using Gallery.GalleryScenes.AssWall;
using Gallery.GalleryScenes.CommonSexNPC;
using Gallery.GalleryScenes.CommonSexPlayer;
using Gallery.GalleryScenes.Daruma;
using Gallery.GalleryScenes.Delivery;
using Gallery.GalleryScenes.ManRapes;
using Gallery.GalleryScenes.ManSleepRape;
using Gallery.GalleryScenes.Onani;
using Gallery.GalleryScenes.PlayerRaped;
using Gallery.GalleryScenes.Slave;
using Gallery.GalleryScenes.Toilet;
using Gallery.GalleryScenes.ToiletNpc;
using YotanModCore;

namespace Gallery
{
	public class GalleryScenesManager
	{
		public static GalleryScenesManager Instance { get; set; }

		private List<IGalleryScene> ActiveScenes = new List<IGalleryScene>();

		private Dictionary<CommonStates, SceneEventHandler> SceneHandlers = new Dictionary<CommonStates, SceneEventHandler>();

		public List<ISceneManager> SceneManagers = new List<ISceneManager>()
		{
			ManRapesSceneManager.Instance,
			DarumaSceneManager.Instance,
			SlaveSceneManager.Instance,
			CommonSexNPCSceneManager.Instance,
			CommonSexPlayerSceneManager.Instance,
			AssWallSceneManager.Instance,
			DeliverySceneManager.Instance,
			ManSleepRapeSceneManager.Instance,
			PlayerRapedSceneManager.Instance,
			new ToiletNpcSceneManager(),
			new ToiletSceneManager(),
		};

		public static void Init() {
			Instance = new GalleryScenesManager();
			if (GameInfo.GameVersion >= GameInfo.ToVersion("0.1.0")) {
				Instance.SceneManagers.Add(new OnaniSceneManager());
			}
		}

		public IGalleryScene GetSceneWithChara(CommonStates chara, Type sceneType = null)
		{
			if (sceneType == null) {
				return this.ActiveScenes.Find((scn) => scn.IsCharacterInScene(chara));
			} else {
				return this.ActiveScenes.Find((scn) => scn.IsCharacterInScene(chara) && scn.GetType() == sceneType);
			}
		}

		public IGalleryScene[] GetScenesByType(Type sceneType) {
			return ActiveScenes.FindAll((scn) => scn.GetType() == sceneType).ToArray();
		}

		public void CheckExistingScenes(CommonStates chara, string scene)
		{
			var sceneWithChara = GetSceneWithChara(chara);
			if (sceneWithChara != null) {
				var charaName = CommonUtils.DetailedString(chara);
				GalleryLogger.LogError($"CheckForExistingScenes: Found existing scene for {charaName} while creating {scene}. Existing: {sceneWithChara}");
			}
		}

		public void EndScene(Type sceneType, CommonStates charaA, CommonStates charaB = null) {
			var sceneA = GetSceneWithChara(charaA, sceneType);
			IGalleryScene sceneB;
			if (charaB != null) {
				sceneB = GetSceneWithChara(charaB, sceneType);
			} else {
				sceneB = sceneA;
			}

			if (sceneA == null) {
				GalleryLogger.LogError($"EndScene ({sceneType}): sceneA == null ({CommonUtils.LogName(charaA)})");
			}

			if (sceneA != sceneB) {
				GalleryLogger.LogError($"EndScene ({sceneType}): sceneA != sceneB: {sceneA} != {sceneB}");
			}

			if (sceneA != null && !sceneA.GetType().IsEquivalentTo(sceneType)) {
				GalleryLogger.LogError($"EndScene ({sceneType}): sceneA.GetType() != sceneType: {sceneA.GetType()} != {sceneType}");
			}

			if (sceneB != null && !sceneB.GetType().IsEquivalentTo(sceneType)) {
				GalleryLogger.LogError($"EndScene ({sceneType}): sceneB.GetType() != sceneType: {sceneB.GetType()} != {sceneType}");
			}

			sceneA?.OnEnd();
			ActiveScenes.Remove(sceneA);
		
			if (sceneA != sceneB) {
				sceneB?.OnEnd();
				ActiveScenes.Remove(sceneB);
			}
		}

		public SceneEventHandler GetSceneHandlerForCommon(CommonStates common)
		{
			return this.SceneHandlers.GetValueOrDefault(common, null);
		}

		public void AddSceneHandlerForCommon(CommonStates common, SceneEventHandler handler)
		{
			if (this.SceneHandlers.ContainsKey(common))
				this.SceneHandlers[common] = handler;
			else
				this.SceneHandlers.Add(common, handler);
		}

		public void RemoveSceneHandlerForCommon(CommonStates common)
		{
			if (this.SceneHandlers.ContainsKey(common))
				this.SceneHandlers.Remove(common);
		}

		public void AddScene(IGalleryScene scene)
		{
			this.CheckExistingScenes(scene.GetCharacter1(), scene.GetType().Name);
			this.CheckExistingScenes(scene.GetCharacter2(), scene.GetType().Name);

			ActiveScenes.Add(scene);
		}
	}
}

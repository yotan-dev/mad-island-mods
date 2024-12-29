using System.Linq;
using YotanModCore;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;
using Gallery.UI;
using static Gallery.GallerySceneInfo;
using YotanModCore.Consts;

namespace Gallery.GalleryScenes.CommonSexNPC
{
	public class CommonSexNPCSceneManager : ISceneManager
	{
		public static readonly CommonSexNPCSceneManager Instance = new CommonSexNPCSceneManager();

		private bool IsUnlocked(int npcA, int npcB, int placeGrade, SexPlace.SexPlaceType placeType, SexManager.SexCountState sexType)
		{
			return GalleryState.Instance.CommonSexNpc.Any((interaction) =>
			{
				return interaction.Character1.Id == npcA
					&& interaction.Character2.Id == npcB
					&& interaction.PlaceGrade == placeGrade
					&& interaction.PlaceType == placeType
					&& interaction.SexType == sexType
					;
			});
		}

		public void Unlock(GalleryChara npcA, GalleryChara npcB, int placeGrade, SexPlace.SexPlaceType placeType, SexManager.SexCountState sexType)
		{
			if (npcA == null || npcB == null)
			{
				GalleryLogger.LogError($"CommonSexNPCScene#Unlock: chara is null");
				return;
			}

			var desc = $"{npcA} x {npcB} (Grade: {placeGrade}, Place Type: {placeType}, Sex Type: {sexType})";

			if (this.IsUnlocked(npcA.Id, npcB.Id, placeGrade, placeType, sexType))
			{
				GalleryLogger.LogDebug($"CommonSexNPCScene#Unlock: already unlocked for {desc}");
				return;
			}

			GalleryLogger.LogDebug($"CommonSexNPCScene#Unlock: event UNLOCKED for {desc}");
			SaveFile.GalleryState.Instance.CommonSexNpc.Add(
				new NpcSexInteractions(npcA, npcB, sexType, placeType, placeGrade)
			);
		}

		private GallerySceneInfo BuildScene(string charGroup, int npcA, int npcB)
		{
			return new GallerySceneInfo() {
				CharGroup = CharGroups.NativeFemale,
				SceneType = SceneTypes.CommonSexNpc,
				Name = "{npcA} sex with\n{npcB}",
				NpcA = new SceneNpc() { NpcID = npcA, Pregnant = false },
				NpcB = new SceneNpc() { NpcID = npcB, Pregnant = false },
				IsUnlocked = this.IsUnlocked(npcA, npcB, 0, SexPlace.SexPlaceType.Normal, SexManager.SexCountState.Normal),
				GetScene = (PlayData data) => {
					var scene = new HFramework.Scenes.CommonSexNPC(data.NpcA, data.NpcB, data.Prop.GetComponent<SexPlace>(), SexManager.SexCountState.Normal);
					scene.Init(new HFramework.GallerySceneController());
					// scene.AddEventHandler(new HFramework.GallerySceneEventHandler());
					
					return scene;
				},
				Prop = "bed_01",
			};
		}

		public GallerySceneInfo[] GetScenes()
		{
			return new GallerySceneInfo[] {
			// ========== Native Female
			this.BuildScene(CharGroups.NativeFemale, NpcID.FemaleNative, NpcID.MaleNative),
			
			// ========== Native Male
			/*
			new GallerySceneInfo() {
				CharGroup = GallerySceneInfo.CharGroups.NativeMale,
				SceneType = GallerySceneInfo.SceneTypes.CommonSexNpc,
				Name = "{npcA} sex with\n{npcB}",
				NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcID.MaleNative, Pregnant = false },
				NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcID.FemaleNative, Pregnant = false },
				IsUnlocked = true, // GalleryMngr.CommonSexNpcCheck,
				Prop = "leafbed",
			},
			new GallerySceneInfo() {
				RequireDLC = true,
				CharGroup = GallerySceneInfo.CharGroups.NativeMale,
				SceneType = GallerySceneInfo.SceneTypes.CommonSexNpc,
				Name = "{npcA} sex with\n{npcB}",
				NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcID.MaleNative, Pregnant = false },
				NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcID.NativeGirl, Pregnant = false },
				IsUnlocked = true, // GalleryMngr.CommonSexNpcCheck,
				Prop = "leafbed",
			},
			new GallerySceneInfo() {
				RequireDLC = true,
				CharGroup = GallerySceneInfo.CharGroups.NativeMale,
				SceneType = GallerySceneInfo.SceneTypes.CommonSexNpc,
				Name = "{npcA} sex with\n{npcB}",
				NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcID.MaleNative, Pregnant = false },
				NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcID.NativeGirl, Pregnant = true },
				IsUnlocked = true, // GalleryMngr.CommonSexNpcCheck,
				Prop = "leafbed",
			},

			// ========== Native Girl
			new GallerySceneInfo() {
				RequireDLC = true,
				CharGroup = GallerySceneInfo.CharGroups.NativeGirl,
				SceneType = GallerySceneInfo.SceneTypes.CommonSexNpc,
				Name = "{npcA} sex with\n{npcB}",
				NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcID.NativeGirl, Pregnant = false },
				NpcB = new GallerySceneInfo.SceneNpc() { NpcID = NpcID.MaleNative, Pregnant = false },
				IsUnlocked = true, // GalleryMngr.CommonSexNpcCheck,
				Prop = "leafbed",
			},
			*/
			};
		}
	}
}

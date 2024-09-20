using System.Linq;
using YotanModCore;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;
using Gallery.UI;

namespace Gallery.GalleryScenes.ToiletNpc
{
	public class ToiletNpcSceneManager : ISceneManager
	{
		private readonly ToiletNpcSceneTracker SceneTracker = new ToiletNpcSceneTracker();

		public ToiletNpcSceneManager()
		{
			SceneTracker.OnUnlock += this.OnUnlock;
		}

		private bool IsUnlocked(int npcA, int npcB, int placeGrade, SexPlace.SexPlaceType placeType)
		{
			return GalleryState.Instance.ToiletNpc.Any((interaction) =>
			{
				return interaction.Character1.Id == npcA
					&& interaction.Character2.Id == npcB
					&& interaction.ToiletGrade == placeGrade
					&& interaction.PlaceType == placeType
					;
			});
		}

		private void OnUnlock(GalleryChara user, GalleryChara target, int placeGrade, SexPlace.SexPlaceType placeType)
		{
			if (user == null || target == null)
			{
				GalleryLogger.LogError($"ToiletNpcScene#Unlock: chara is null");
				return;
			}

			var desc = $"{user} x {target} (Grade: {placeGrade}, Place Type: {placeType})";

			if (this.IsUnlocked(user.Id, target.Id, placeGrade, placeType))
			{
				GalleryLogger.LogDebug($"ToiletNpcScene#Unlock: already unlocked for {desc}");
				return;
			}

			GalleryLogger.LogDebug($"ToiletNpcScene#Unlock: event UNLOCKED for {desc}");
			SaveFile.GalleryState.Instance.ToiletNpc.Add(
				new ToiletNPCInteraction(user, target, placeType, placeGrade)
			);
		}

		public GallerySceneInfo[] GetScenes()
		{
			// @TODO:
			return new GallerySceneInfo[0];
		}
	}
}
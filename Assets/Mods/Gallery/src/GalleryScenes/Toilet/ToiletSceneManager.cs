using System.Linq;
using YotanModCore;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;
using Gallery.UI;

namespace Gallery.GalleryScenes.Toilet
{
	public class ToiletSceneManager : ISceneManager
	{
		private readonly ToiletSceneTracker SceneTracker = new ToiletSceneTracker();

		public ToiletSceneManager()
		{
			SceneTracker.OnUnlock += this.OnUnlock;
		}

		private bool IsUnlocked(int userNpcId, int targetNpcId, int toiletSize, InventorySlot.Type toiletType)
		{
			return GalleryState.Instance.Toilet.Any((interaction) =>
			{
				return interaction.Character1.Id == userNpcId
					&& interaction.Character2.Id == targetNpcId
					&& interaction.ToiletSize == toiletSize
					&& interaction.ToiletType == toiletType
					;
			});
		}

		private void OnUnlock(GalleryChara user, GalleryChara target, int toiletsize, InventorySlot.Type toiletType)
		{
			if (user == null || target == null)
			{
				GalleryLogger.LogError($"ToiletNpcScene#Unlock: chara is null");
				return;
			}

			var desc = $"{user} x {target} (Size: {toiletsize}, Type: {toiletType})";

			if (this.IsUnlocked(user.Id, target.Id, toiletsize, toiletType))
			{
				GalleryLogger.LogDebug($"ToiletNpcScene#Unlock: already unlocked for {desc}");
				return;
			}

			GalleryLogger.LogDebug($"ToiletNpcScene#Unlock: event UNLOCKED for {desc}");
			SaveFile.GalleryState.Instance.Toilet.Add(
				new ToiletInteractions(user, target, toiletType, toiletsize)
			);
		}

		public GallerySceneInfo[] GetScenes()
		{
			// @TODO:
			return new GallerySceneInfo[0];
		}
	}
}
using System.Linq;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Toilet
{
	public class ToiletSceneManager : ISceneManager
	{
		public static readonly ToiletSceneManager Instance = new ToiletSceneManager();

		private ToiletSceneManager() { }

		private bool IsUnlocked(int userNpcId, int targetNpcId)
		{
			return GalleryState.Instance.Toilet.Any((interaction) =>
			{
				return interaction.Character1.Id == userNpcId
					&& interaction.Character2.Id == targetNpcId
					;
			});
		}

		public void Unlock(GalleryChara user, GalleryChara target)
		{
			if (user == null || target == null)
			{
				GalleryLogger.LogError($"ToiletNpcScene#Unlock: chara is null");
				return;
			}

			var desc = $"{user} x {target}";

			if (this.IsUnlocked(user.Id, target.Id))
			{
				GalleryLogger.LogDebug($"ToiletNpcScene#Unlock: already unlocked for {desc}");
				return;
			}

			GalleryLogger.LogDebug($"ToiletNpcScene#Unlock: event UNLOCKED for {desc}");
			SaveFile.GalleryState.Instance.Toilet.Add(
				new ToiletInteractions(user, target)
			);
		}

		public GallerySceneInfo[] GetScenes()
		{
			// @TODO:
			return new GallerySceneInfo[0];
		}
	}
}
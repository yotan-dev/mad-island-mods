using System.Linq;
using YotanModCore;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;
using Gallery.UI;

namespace Gallery.GalleryScenes.Delivery
{
	public class DeliverySceneManager : ISceneManager
	{
		private readonly DeliverySceneTracker SceneTracker = new DeliverySceneTracker();

		public DeliverySceneManager()
		{
			SceneTracker.OnUnlock += this.OnUnlock;
		}

		private bool IsUnlocked(int npcA)
		{
			return GalleryState.Instance.Delivery.Any((interaction) =>
			{
				return interaction.Character1.Id == npcA;
			});
		}

		private void OnUnlock(GalleryChara girl)
		{
			if (girl == null)
			{
				GalleryLogger.LogError($"DeliverySceneManager#Unlock: chara is null");
				return;
			}

			if (this.IsUnlocked(girl.Id))
			{
				GalleryLogger.LogDebug($"DeliverySceneManager#Unlock: already unlocked for {girl}");
				return;
			}

			GalleryLogger.LogDebug($"DeliverySceneManager#Unlock: event UNLOCKED for {girl}");
			SaveFile.GalleryState.Instance.Delivery.Add(
				new SelfInteraction(girl)
			);
		}

		public GallerySceneInfo[] GetScenes()
		{
			// @TODO:
			return new GallerySceneInfo[0];
		}
	}
}
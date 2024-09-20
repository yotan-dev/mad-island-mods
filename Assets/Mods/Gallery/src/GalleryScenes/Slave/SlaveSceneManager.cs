using System.Linq;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;
using Gallery.UI;

namespace Gallery.GalleryScenes.Slave
{
	public class SlaveSceneManager : ISceneManager
	{
		private readonly SlaveSceneTracker SceneTracker = new SlaveSceneTracker();

		public SlaveSceneManager()
		{
			SceneTracker.OnUnlock += this.OnUnlock;
		}

		private bool IsUnlocked(int npcA, int npcB)
		{
			return GalleryState.Instance.AssWall.Any((interaction) =>
			{
				return interaction.Character1.Id == npcA
					&& interaction.Character2.Id == npcB
					;
			});
		}

		private void OnUnlock(GalleryChara player, GalleryChara girl)
		{
			if (player == null || girl == null)
			{
				GalleryLogger.LogError($"SlaveSceneManager#Unlock: chara is null");
				return;
			}

			var desc = $"{player} x {girl}";

			if (this.IsUnlocked(player.Id, girl.Id))
			{
				GalleryLogger.LogDebug($"SlaveSceneManager#Unlock: already unlocked for {desc}");
				return;
			}

			GalleryLogger.LogDebug($"SlaveSceneManager#Unlock: event UNLOCKED for {desc}");
			SaveFile.GalleryState.Instance.Slave.Add(
				new CharacterInteraction(player, girl)
			);
		}

		public GallerySceneInfo[] GetScenes()
		{
			return new GallerySceneInfo[] {
				// @TODO: add more scenes
			};
		}
	}
}
using System.Linq;
using YotanModCore;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;
using Gallery.UI;

namespace Gallery.GalleryScenes.Onani
{
	public class OnaniSceneManager : ISceneManager
	{
		private readonly OnaniSceneTracker SceneTracker = new OnaniSceneTracker();

		public OnaniSceneManager()
		{
			SceneTracker.OnUnlock += this.OnUnlock;
		}

		private bool IsUnlocked(int npc, bool perfume)
		{
			return GalleryState.Instance.Onani.Any((interaction) =>
			{
				return interaction.Character1.Id == npc
					&& interaction.Perfume == perfume
					;
			});
		}

		private void OnUnlock(GalleryChara npc, bool perfume)
		{
			if (npc == null)
			{
				GalleryLogger.LogError($"OnaniSceneManager#Unlock: chara is null");
				return;
			}

			var desc = $"{npc} (Perfume: {perfume})";

			if (this.IsUnlocked(npc.Id, perfume))
			{
				GalleryLogger.LogDebug($"OnaniSceneManager#Unlock: already unlocked for {desc}");
				return;
			}

			GalleryLogger.LogDebug($"OnaniSceneManager#Unlock: event UNLOCKED for {desc}");
			SaveFile.GalleryState.Instance.Onani.Add(
				new OnaniInteraction(npc, perfume)
			);
		}

		// private GallerySceneInfo BuildSceneInfo(int npcB, bool dlc = false)
		// {
			
		// }

		public GallerySceneInfo[] GetScenes()
		{
			return new GallerySceneInfo[] {
				// >= 1.0.0
				// this.BuildSceneInfo(NpcID.FemaleNative, perfume: false),
				// this.BuildSceneInfo(NpcID.FemaleNative, perfume: true),
			};
		}
	}
}
using System.Linq;
using YotanModCore;
using Gallery.SaveFile.Containers;
using Gallery.UI;

namespace Gallery.GalleryScenes.ManSleepRape
{
	public class ManSleepRapeSceneManager : ISceneManager
	{
		private readonly ManSleepRapeSceneTracker Tracker = new ManSleepRapeSceneTracker();

		public ManSleepRapeSceneManager()
		{
			this.Tracker.OnUnlock += this.OnUnlock;
		}

		private bool IsUnlocked(int man, int girl, ManRapesSexType mode) {
			return SaveFile.GalleryState.Instance.SleepRapes.Any((interaction) => {
				return interaction.Character1.Id == man &&
					interaction.Character2.Id == girl
					&& interaction.SexType == mode;
			});
		}

		public GallerySceneInfo[] GetScenes()
		{
			// @TODO:
			return new GallerySceneInfo[0];
		}

		private void OnUnlock(GalleryChara man, GalleryChara girl, ManRapesSexType mode)
		{
			if (IsUnlocked(man.Id, girl.Id, mode)) {
				GalleryLogger.LogDebug($"ManSleepRapeSceneManager: OnEnd: event already unlocked for {man} x {girl} (mode: {mode})");
				return;
			}

			GalleryLogger.LogDebug($"ManSleepRapeSceneManager: OnEnd: event UNLOCKED for {man} x {girl} (mode: {mode})");
			SaveFile.GalleryState.Instance.SleepRapes.Add(new SexTypeInteraction<ManRapesSexType>(man, girl, mode));
		}
	}
}
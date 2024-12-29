using System.Linq;
using Gallery.SaveFile.Containers;
using HFramework.Scenes;

namespace Gallery.GalleryScenes.ManSleepRape
{
	public class ManSleepRapeSceneManager : ISceneManager
	{
		public static readonly ManSleepRapeSceneManager Instance = new ManSleepRapeSceneManager();

		private ManSleepRapeSceneManager()
		{

		}

		private bool IsUnlocked(int man, int girl, ManRapeSleepState mode)
		{
			return SaveFile.GalleryState.Instance.SleepRapes.Any((interaction) =>
			{
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

		public void Unlock(GalleryChara man, GalleryChara girl, ManRapeSleepState mode)
		{
			if (IsUnlocked(man.Id, girl.Id, mode))
			{
				GalleryLogger.LogDebug($"ManSleepRapeSceneManager: OnEnd: event already unlocked for {man} x {girl} (mode: {mode})");
				return;
			}

			GalleryLogger.LogDebug($"ManSleepRapeSceneManager: OnEnd: event UNLOCKED for {man} x {girl} (mode: {mode})");
			SaveFile.GalleryState.Instance.SleepRapes.Add(new SexTypeInteraction<ManRapeSleepState>(man, girl, mode));
		}
	}
}

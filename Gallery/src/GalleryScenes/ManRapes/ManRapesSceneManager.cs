using System.Linq;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.ManRapes
{
	public class ManRapesSceneManager : ISceneManager
	{
		public static readonly ManRapesSceneManager Instance = new ManRapesSceneManager();

		private ManRapesSceneManager()
		{
			
		}

		private bool IsUnlocked(int man, int girl, bool isPregnant) {
			return SaveFile.GalleryState.Instance.ManRapes.Any((interaction) => {
				return interaction.Character1.Id == man
					&& interaction.Character2.Id == girl
					&& interaction.Character2.IsPregnant == isPregnant;
			});
		}

		// private GallerySceneInfo BuildSceneInfo(
		// 	string charGroup,
		// 	int npcB,
		// 	bool isFainted = false,
		// 	bool dlc = false,
		// 	int minVersion = 0,
		// 	bool isPregnant = false
		// )
		// {
		// 	string name = "{npcA} rapes\n{npcB}";
		// 	if (isPregnant)
		// 		name += " (Pregnant)";

		// 	return new GallerySceneInfo() {
		// 		CharGroup = charGroup,
		// 		SceneType = SceneTypes.ManRapes,
		// 		Name = name,
		// 		NpcA = new SceneNpc() { NpcID = NpcID.Man, Pregnant = false },
		// 		NpcB = new SceneNpc() { NpcID = npcB, Pregnant = isPregnant },
		// 		IsUnlocked = this.IsUnlocked(NpcID.Man, npcB, isFainted, isPregnant),
		// 		RequireDLC = dlc,
		// 		MinVersion = minVersion,
		// 		Play = (PlayData data) => this.Player.Play(data.NpcA, data.NpcB),
		// 	};
		// }

		public GallerySceneInfo[] GetScenes()
		{
			return new GallerySceneInfo[] {
				// this.BuildSceneInfo(CharGroups.Yona, NpcID.Yona, isFainted: false, minVersion: GameInfo.ToVersion("0.1.4")),
				// this.BuildSceneInfo(CharGroups.NativeFemale, NpcID.FemaleNative, isFainted: false),
				// this.BuildSceneInfo(CharGroups.NativeFemale, NpcID.FemaleNative, isFainted: false, isPregnant: false, minVersion: GameInfo.ToVersion("0.1.7")),
				// this.BuildSceneInfo(CharGroups.NativeGirl, NpcID.NativeGirl, isFainted: false, dlc: true),
				// this.BuildSceneInfo(CharGroups.UndergroundWoman, NpcID.UnderGroundWoman, isFainted: false),
				// this.BuildSceneInfo(CharGroups.Mummy, NpcID.Mummy, isFainted: false),
			};
		}

		public void Unlock(GalleryChara man, GalleryChara girl)
		{
			if (IsUnlocked(man.Id, girl.Id, girl.IsPregnant)) {
				GalleryLogger.LogDebug($"ManRapesSceneManager: OnEnd: event already unlocked for {man} x {girl}");
				return;
			}

			GalleryLogger.LogDebug($"ManRapesSceneManager: OnEnd: event UNLOCKED for {man} x {girl}");
			SaveFile.GalleryState.Instance.ManRapes.Add(new ManRapesInteraction(man, girl));
		}
	}
}
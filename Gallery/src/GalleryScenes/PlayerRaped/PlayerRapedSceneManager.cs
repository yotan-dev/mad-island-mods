using System.Linq;
using YotanModCore;
using Gallery.SaveFile.Containers;
using YotanModCore.Consts;
using static Gallery.GallerySceneInfo;

namespace Gallery.GalleryScenes.PlayerRaped
{
	public class PlayerRapedSceneManager : ISceneManager
	{
		public static readonly PlayerRapedSceneManager Instance = new PlayerRapedSceneManager();

		private PlayerRapedSceneManager() { }

		private bool IsUnlocked(int playerNpcId, int rapistNpcId)
		{
			return SaveFile.GalleryState.Instance.PlayerRaped.Any((interaction) =>
			{
				return interaction.Character1.Id == playerNpcId &&
					interaction.Character2.Id == rapistNpcId;
			});
		}

		private GallerySceneInfo BuildYonaSceneInfo(int npcB, bool dlc = false)
		{
			return new GallerySceneInfo()
			{
				CharGroup = CharGroups.Yona,
				SceneType = SceneTypes.PlayerRaped,
				Name = "{npcB} rapes\n{npcA}",
				NpcA = new SceneNpc() { NpcID = NpcID.Yona, Pregnant = false },
				NpcB = new SceneNpc() { NpcID = npcB, Pregnant = false },
				IsUnlocked = this.IsUnlocked(NpcID.Yona, npcB),
				RequireDLC = dlc,
				GetScene = (PlayData data) =>
				{
					var scene = new ExtendedHSystem.Scenes.PlayerRaped(data.NpcA, data.NpcB);
					scene.Init(new ExtendedHSystem.GallerySceneController());
					scene.AddEventHandler(new ExtendedHSystem.GallerySceneEventHandler());

					return scene;
				},
			};
		}

		private GallerySceneInfo BuildManSceneInfo(string charGroup, int npcB, bool dlc = false)
		{
			return new GallerySceneInfo()
			{
				CharGroup = charGroup,
				SceneType = SceneTypes.PlayerRaped,
				Name = "{npcB} rapes\n{npcA}",
				NpcA = new SceneNpc() { NpcID = NpcID.Man, Pregnant = false },
				NpcB = new SceneNpc() { NpcID = npcB, Pregnant = false },
				IsUnlocked = this.IsUnlocked(NpcID.Man, npcB),
				RequireDLC = dlc,
				GetScene = (PlayData data) =>
				{
					var scene = new ExtendedHSystem.Scenes.PlayerRaped(data.NpcA, data.NpcB);
					scene.Init(new ExtendedHSystem.GallerySceneController());
					scene.AddEventHandler(new ExtendedHSystem.GallerySceneEventHandler());

					return scene;
				},
			};
		}

		public GallerySceneInfo[] GetScenes()
		{
			return new GallerySceneInfo[] {
				this.BuildYonaSceneInfo(NpcID.MaleNative),
				this.BuildYonaSceneInfo(NpcID.BigNative),
				this.BuildYonaSceneInfo(NpcID.Werewolf),
				this.BuildYonaSceneInfo(NpcID.Spike),
				this.BuildYonaSceneInfo(NpcID.BossNative),
				this.BuildYonaSceneInfo(NpcID.Oldguy),
				this.BuildManSceneInfo(CharGroups.Mummy, NpcID.Mummy),
				this.BuildManSceneInfo(CharGroups.FemaleLargeNative, NpcID.FemaleLargeNative),
			};
		}

		public void Unlock(GalleryChara player, GalleryChara rapist)
		{
			if (IsUnlocked(player.Id, rapist.Id))
			{
				GalleryLogger.LogDebug($"PlayerRapedSceneManager: OnEnd: event already unlocked for {player} x {rapist}");
				return;
			}

			GalleryLogger.LogDebug($"PlayerRapedSceneManager: OnEnd: event UNLOCKED for {player} x {rapist}");
			SaveFile.GalleryState.Instance.PlayerRaped.Add(new CharacterInteraction(player, rapist));
		}
	}
}
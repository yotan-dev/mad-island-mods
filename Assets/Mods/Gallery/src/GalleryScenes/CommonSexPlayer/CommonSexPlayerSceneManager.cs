using System.Linq;
using YotanModCore;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;
using YotanModCore.Consts;
using static Gallery.GallerySceneInfo;

namespace Gallery.GalleryScenes.CommonSexPlayer
{
	public class CommonSexPlayerSceneManager : ISceneManager
	{
		private readonly CommonSexPlayerSceneTracker SceneTracker = new CommonSexPlayerSceneTracker();

		public CommonSexPlayerSceneManager()
		{
			SceneTracker.OnUnlock += this.OnUnlock;
		}

		private bool IsUnlocked(int player, bool playerPregnant, int npc, bool npcPregnant, int sexType, int specialFlag)
		{
			return GalleryState.Instance.CommonSexPlayer.Any((interaction) =>
			{
				return interaction.Character1.Id == player
					&& interaction.Character1.IsPregnant == playerPregnant
					&& interaction.Character2.Id == npc
					&& interaction.Character2.IsPregnant == npcPregnant
					&& interaction.SexType == sexType
					&& interaction.SpecialFlag == specialFlag
					;
			});
		}

		private bool IsUnlocked(int player, bool playerPregnant, int npc, bool npcPregnant, int specialFlag)
		{
			return GalleryState.Instance.CommonSexPlayer.Any((interaction) =>
			{
				return interaction.Character1.Id == player
					&& interaction.Character1.IsPregnant == playerPregnant
					&& interaction.Character2.Id == npc
					&& interaction.Character2.IsPregnant == npcPregnant
					// && interaction.SexType == sexType // I think this doesn't matter
					&& interaction.SpecialFlag == specialFlag
					;
			});
		}

		private void OnUnlock(GalleryChara player, GalleryChara npc, int sexType, int specialFlag)
		{
			if (player == null || npc == null)
			{
				GalleryLogger.LogError($"CommonSexPlayerSceneManager#Unlock: chara is null");
				return;
			}

			var desc = $"{player} x {npc} (SexType: {sexType} / SpecialFlag: {specialFlag})";

			if (this.IsUnlocked(player.Id, player.IsPregnant, npc.Id, npc.IsPregnant, sexType, specialFlag))
			{
				GalleryLogger.LogDebug($"CommonSexPlayerSceneManager#Unlock: already unlocked for {desc}");
				return;
			}

			GalleryLogger.LogDebug($"CommonSexPlayerSceneManager#Unlock: event UNLOCKED for {desc}");
			SaveFile.GalleryState.Instance.CommonSexPlayer.Add(
				new CommonSexPlayerInteraction(player, npc, sexType, specialFlag)
			);
		}

		private GallerySceneInfo BuildSceneInfo(
			string charGroup,
			int npc,
			int player,
			int sexType,
			bool playerPregnant = false,
			bool npcPregnant = false,
			bool dlc = false,
			int minVersion = 0
		)
		{
			var npcAName = CommonUtils.GetName(npc);
			if (playerPregnant)
				npcAName += " (Pregnant)";

			var npcBName = CommonUtils.GetName(player);
			if (npcPregnant)
				npcBName += " (Pregnant)";

			return new GallerySceneInfo() {
				CharGroup = charGroup,
				SceneType = SceneTypes.CommonSexPlayer,
				Name = $"{npcAName} sex with\n{npcBName}\n(SexType: {sexType})",
				NpcA = new SceneNpc() { NpcID = npc, Pregnant = npcPregnant },
				NpcB = new SceneNpc() { NpcID = player, Pregnant = playerPregnant },
				IsUnlocked = this.IsUnlocked(player, playerPregnant, npc, npcPregnant, sexType, 0),
				MinVersion = minVersion,
				RequireDLC = dlc,
				Play = (PlayData data) => {
					// NpcB is player, and after v0.0.12 player is also sent first
					if (GameInfo.GameVersion <= GameInfo.ToVersion("0.0.12"))
						return Managers.mn.sexMN.CommonSexPlayer(0, data.NpcA, data.NpcB, Managers.mn.sexMN.transform.position, 0);
					else
						return Managers.mn.sexMN.CommonSexPlayer(0, data.NpcB, data.NpcA, Managers.mn.sexMN.transform.position, 0);
				},
			};
		}

		public GallerySceneInfo[] GetScenes()
		{
			return new GallerySceneInfo[] {
				// Man X NPC
				this.BuildSceneInfo(CharGroups.NativeFemale, NpcID.FemaleNative, NpcID.Man, 0),
				this.BuildSceneInfo(CharGroups.NativeFemale, NpcID.FemaleNative, NpcID.Man, 0, npcPregnant: true),
				this.BuildSceneInfo(CharGroups.NativeGirl, NpcID.NativeGirl, NpcID.Man, 0, dlc: true),
				this.BuildSceneInfo(CharGroups.UndergroundWoman, NpcID.UnderGroundWoman, NpcID.Man, 0),
				this.BuildSceneInfo(CharGroups.Giant, NpcID.Giant, NpcID.Man, 0),
				this.BuildSceneInfo(CharGroups.Mermaid, NpcID.Mermaid, NpcID.Man, 0),
				this.BuildSceneInfo(CharGroups.Sally, NpcID.Sally, NpcID.Man, 0),
				new() {
					CharGroup = CharGroups.Shino,
					SceneType = SceneTypes.CommonSexPlayer,
					Name = "{npcA} sex with\n{npcB}\n(Titty fuck)",
					NpcA = new SceneNpc() { NpcID = NpcID.Shino, Pregnant = false },
					NpcB = new SceneNpc() { NpcID = NpcID.Man, Pregnant = false },
					IsUnlocked = this.IsUnlocked(NpcID.Man, false, NpcID.Shino, false, 1),
					RequireDLC = false,
					Play = (PlayData data) => {
						// NpcB is player, and after v0.0.12 player is also sent first
						if (GameInfo.GameVersion <= GameInfo.ToVersion("0.0.12"))
							return Managers.mn.sexMN.CommonSexPlayer(0, data.NpcA, data.NpcB, Managers.mn.sexMN.transform.position, 0);
						else
							return Managers.mn.sexMN.CommonSexPlayer(0, data.NpcB, data.NpcA, Managers.mn.sexMN.transform.position, 0);
					},
				},
				new() {
					CharGroup = CharGroups.Shino,
					SceneType = SceneTypes.CommonSexPlayer,
					Name = "{npcA} sex with\n{npcB}\n(Sex)",
					NpcA = new SceneNpc() { NpcID = NpcID.Shino, Pregnant = false },
					NpcB = new SceneNpc() { NpcID = NpcID.Man, Pregnant = false },
					IsUnlocked = this.IsUnlocked(NpcID.Man, false, NpcID.Shino, false, 0),
					RequireDLC = false,
					Play = (PlayData data) => {
						// NpcB is player, and after v0.0.12 player is also sent first
						if (GameInfo.GameVersion <= GameInfo.ToVersion("0.0.12"))
							return Managers.mn.sexMN.CommonSexPlayer(0, data.NpcA, data.NpcB, Managers.mn.sexMN.transform.position, 1);
						else
							return Managers.mn.sexMN.CommonSexPlayer(0, data.NpcB, data.NpcA, Managers.mn.sexMN.transform.position, 1);
					},
				},

				// Yona X NPC
				this.BuildSceneInfo(CharGroups.Yona, NpcID.MaleNative, NpcID.Yona, 0),
				this.BuildSceneInfo(CharGroups.Yona, NpcID.SmallNative, NpcID.Yona, 0, minVersion: GameInfo.ToVersion("0.1.4")),
				this.BuildSceneInfo(CharGroups.Yona, NpcID.ElderBrotherNative, NpcID.Yona, 0, minVersion: GameInfo.ToVersion("0.1.7")),
			};
		}
	}
}
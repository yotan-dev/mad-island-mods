#nullable enable

using System.Collections;
using System.Linq;
using Gallery.ConfigFiles;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.PlayerRaped
{
	public class PlayerRapedController : BaseController
	{
		public override void Unlock(string performerId, GalleryChara[] charas)
		{
			if (charas.Length < 2)
			{
				PLogger.LogError($"PlayerRapedController: Not enough actors. Expected 2, got {charas.Length}");
				return;
			}

			if (!this.EnsurePerformer(performerId))
				return;

			var desc = $"{charas[0]} x {charas[1]}";

			var actors = new GalleryActor[2] { new(charas[0]), new(charas[1]) };
			if (this.IsUnlocked(actors))
			{
				PLogger.LogInfo($"PlayerRapedController: Already unlocked: {desc}");
				return;
			}

			PLogger.LogInfo($"PlayerRapedController: Unlocking: {desc}");
			SaveFile.GalleryState.Instance.PlayerRaped.Add(
				new CharacterInteraction(performerId, charas[0], charas[1])
			);
		}

		public override bool IsUnlocked(GalleryActor[] actors)
		{
			if (actors.Length < 2)
			{
				PLogger.LogError($"PlayerRapedController: Not enough actors. Expected 2, got {actors.Length}");
				return false;
			}

			return GalleryState.Instance.PlayerRaped.Any((interaction) =>
			{
				return interaction.PerformerId == this.PerformerId
					&& interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character2.Id == actors[1].NpcId
					;
			});
		}

		public override bool IsUnlocked(string performerId, GalleryActor[] actors)
		{
			if (actors.Length < 2)
			{
				PLogger.LogError($"PlayerRapedController: Not enough actors. Expected 2, got {actors.Length}");
				return false;
			}

			return GalleryState.Instance.PlayerRaped.Any((interaction) =>
			{
				return interaction.PerformerId == performerId
					&& interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character2.Id == actors[1].NpcId
					;
			});
		}

		protected override IEnumerator GetScene(PlayData playData)
		{
			if (playData.Actors.Count < 2)
			{
				PLogger.LogError($"PlayerRapedController: Not enough actors. Expected 2, got {playData.Actors.Count}");
				yield break;
			}

			this.Scene = new HFramework.Scenes.PlayerRaped(playData.Actors[0], playData.Actors[1]);
		}
	}
}

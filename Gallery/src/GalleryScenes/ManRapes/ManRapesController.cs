#nullable enable

using System.Collections;
using System.Linq;
using Gallery.ConfigFiles;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.ManRapes
{
	public class ManRapesController : BaseController
	{
		public int SexType;
		
		public int SpecialFlag;

		public override void Unlock(string performerId, GalleryChara[] charas)
		{
			if (charas.Length < 2)
			{
				PLogger.LogError($"ManRapesController: Not enough actors. Expected 2, got {charas.Length}");
				return;
			}

			if (!this.EnsurePerformer(performerId))
				return;

			var desc = $"{charas[0]} x {charas[1]} (Sex Type: {this.SexType}, Special Flag: {this.SpecialFlag})";

			var actors = new GalleryActor[2] { new(charas[0]), new(charas[1]) };
			if (this.IsUnlocked(actors))
			{
				PLogger.LogInfo($"ManRapesController: Already unlocked: {desc}");
				return;
			}

			PLogger.LogInfo($"ManRapesController: Unlocking: {desc}");
			SaveFile.GalleryState.Instance.ManRapes.Add(
				new ManRapesInteraction(performerId, charas[0], charas[1])
			);
		}

		public override bool IsUnlocked(GalleryActor[] actors)
		{
			if (actors.Length < 2)
			{
				PLogger.LogError($"ManRapesController: Not enough actors. Expected 2, got {actors.Length}");
				return false;
			}

			return GalleryState.Instance.ManRapes.Any((interaction) =>
			{
				return interaction.PerformerId == this.PerformerId
					&& interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character2.Id == actors[1].NpcId
					&& interaction.Character2.IsPregnant == actors[1].Pregnant
					&& interaction.Character2.IsFainted == actors[1].Fainted
					;
			});
		}

		public override bool IsUnlocked(string performerId, GalleryActor[] actors)
		{
			if (actors.Length < 2)
			{
				PLogger.LogError($"ManRapesController: Not enough actors. Expected 2, got {actors.Length}");
				return false;
			}

			return SaveFile.GalleryState.Instance.ManRapes.Any((interaction) =>
			{
				return interaction.PerformerId == performerId
					&& interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character2.Id == actors[1].NpcId
					&& interaction.Character2.IsPregnant == actors[1].Pregnant
					&& interaction.Character2.IsFainted == actors[1].Fainted
					;
			});
		}

		protected override IEnumerator GetScene(PlayData playData)
		{
			if (playData.Actors.Count < 2)
			{
				PLogger.LogError($"ManRapesController: Not enough actors. Expected 2, got {playData.Actors.Count}");
				yield break;
			}

			this.Scene = new HFramework.Scenes.ManRapes(playData.Actors[1], playData.Actors[0]);
		}
	}
}

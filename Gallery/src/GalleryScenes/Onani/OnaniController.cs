#nullable enable

using System.Collections;
using System.Linq;
using Gallery.ConfigFiles;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Onani
{
	public class OnaniController : BaseController
	{
		public bool Perfume { get; set; }

		public override void Unlock(string performerId, GalleryChara[] charas)
		{
			if (charas.Length < 1)
			{
				PLogger.LogError($"OnaniController: Not enough actors. Expected 1, got {charas.Length}");
				return;
			}

			if (!this.EnsurePerformer(performerId))
				return;

			var desc = $"{charas[0]}";

			var actors = new GalleryActor[] { new(charas[0]) };
			if (this.IsUnlocked(actors))
			{
				PLogger.LogInfo($"OnaniController: Already unlocked: {desc}");
				return;
			}

			PLogger.LogInfo($"OnaniController: Unlocking: {desc}");
			SaveFile.GalleryState.Instance.Onani.Add(
				new OnaniInteraction(performerId, charas[0], this.Perfume)
			);
		}

		public override bool IsUnlocked(GalleryActor[] actors)
		{
			if (actors.Length < 1)
			{
				PLogger.LogError($"OnaniController: Not enough actors. Expected 1, got {actors.Length}");
				return false;
			}

			return GalleryState.Instance.Onani.Any((interaction) =>
			{
				return interaction.PerformerId == this.PerformerId
					&& interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character1.IsPregnant == actors[0].Pregnant
					&& interaction.Perfume == this.Perfume
					;
			});
		}

		public override bool IsUnlocked(string performerId, GalleryActor[] actors)
		{
			if (actors.Length < 1)
			{
				PLogger.LogError($"OnaniController: Not enough actors. Expected 1, got {actors.Length}");
				return false;
			}

			return SaveFile.GalleryState.Instance.Onani.Any((interaction) =>
			{
				return interaction.PerformerId == performerId
					&& interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character1.IsPregnant == actors[0].Pregnant
					&& interaction.Perfume == this.Perfume
					;
			});
		}

		protected override IEnumerator GetScene(PlayData playData)
		{
			if (playData.Actors.Count < 1)
			{
				PLogger.LogError($"OnaniController: Not enough actors. Expected 1, got {playData.Actors.Count}");
				yield break;
			}

			this.Scene = new HFramework.Scenes.OnaniNPC(playData.Actors[0], null, 0f);
		}
	}
}

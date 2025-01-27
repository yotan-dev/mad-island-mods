#nullable enable

using System.Collections;
using System.Linq;
using Gallery.ConfigFiles;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Delivery
{
	public class DeliveryController : BaseController
	{
		public override void Unlock(GalleryChara[] charas)
		{
			if (charas.Length < 1)
			{
				PLogger.LogError($"DeliveryController: Not enough actors. Expected 1, got {charas.Length}");
				return;
			}

			var desc = $"{charas[0]}";

			var actors = new GalleryActor[] { new(charas[0]) };
			if (this.IsUnlocked(actors))
			{
				PLogger.LogInfo($"DeliveryController: Already unlocked: {desc}");
				return;
			}

			PLogger.LogInfo($"DeliveryController: Unlocking: {desc}");
			SaveFile.GalleryState.Instance.Delivery.Add(
				new SelfInteraction(charas[0])
			);
		}

		public override bool IsUnlocked(GalleryActor[] actors)
		{
			if (actors.Length < 1)
			{
				PLogger.LogError($"DeliveryController: Not enough actors. Expected 1, got {actors.Length}");
				return false;
			}

			return GalleryState.Instance.Delivery.Any((interaction) =>
			{
				return interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character1.IsPregnant == actors[0].Pregnant
					;
			});
		}

		protected override IEnumerator GetScene(PlayData playData)
		{
			if (playData.Actors.Count < 1)
			{
				PLogger.LogError($"DeliveryController: Not enough actors. Expected 1, got {playData.Actors.Count}");
				yield break;
			}

			this.Scene = new HFramework.Scenes.Delivery(playData.Actors[0], null, null);
		}
	}
}

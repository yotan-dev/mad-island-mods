#nullable enable

using System.Collections;
using System.Linq;
using Gallery.ConfigFiles;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.CommonSexNPC
{
	public class CommonSexNPCController : BaseController
	{
		public int PlaceGrade;

		public SexPlace.SexPlaceType PlaceType;

		public SexManager.SexCountState SexType;

		public override void Unlock(string performerId, GalleryChara[] charas)
		{
			if (charas.Length < 2)
			{
				PLogger.LogError($"CommonSexNPCController: Not enough actors. Expected 2, got {charas.Length}");
				return;
			}

			if (!this.EnsurePerformer(performerId))
				return;

			var desc = $"{charas[0]} x {charas[1]} (Grade: {this.PlaceGrade}, Place Type: {this.PlaceType}, Sex Type: {this.SexType})";

			var actors = new GalleryActor[2] { new(charas[0]), new(charas[1]) };
			if (this.IsUnlocked(actors))
			{
				PLogger.LogInfo($"CommonSexNPCController: Already unlocked: {desc}");
				return;
			}

			PLogger.LogInfo($"CommonSexNPCController: Unlocking: {desc}");
			SaveFile.GalleryState.Instance.CommonSexNpc.Add(
				new NpcSexInteractions(performerId, charas[0], charas[1], this.SexType, this.PlaceType, this.PlaceGrade)
			);
		}

		public override bool IsUnlocked(GalleryActor[] actors)
		{
			if (actors.Length < 2)
			{
				PLogger.LogError($"CommonSexNPCController: Not enough actors. Expected 2, got {actors.Length}");
				return false;
			}

			return GalleryState.Instance.CommonSexNpc.Any((interaction) =>
			{
				return interaction.PerformerId == this.PerformerId
					&& interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character2.Id == actors[1].NpcId
					&& interaction.PlaceGrade == this.PlaceGrade
					&& interaction.PlaceType == this.PlaceType
					&& interaction.SexType == this.SexType
					;
			});
		}

		public override bool IsUnlocked(string performerId, GalleryActor[] actors)
		{
			if (actors.Length < 2)
			{
				PLogger.LogError($"CommonSexNPCController: Not enough actors. Expected 2, got {actors.Length}");
				return false;
			}

			return GalleryState.Instance.CommonSexNpc.Any((interaction) =>
			{
				return interaction.PerformerId == performerId
					&& interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character2.Id == actors[1].NpcId
					&& interaction.PlaceGrade == this.PlaceGrade
					&& interaction.PlaceType == this.PlaceType
					&& interaction.SexType == this.SexType
					;
			});
		}

		protected override IEnumerator GetScene(PlayData playData)
		{
			if (playData.Actors.Count < 2)
			{
				PLogger.LogError($"CommonSexNPCController: Not enough actors. Expected 2, got {playData.Actors.Count}");
				yield break;
			}

			this.Scene = new HFramework.Scenes.CommonSexNPC(playData.Actors[0], playData.Actors[1], playData.Prop.GetComponent<SexPlace>(), this.SexType);
		}
	}
}

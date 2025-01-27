#nullable enable

using System;
using System.Collections;
using System.Linq;
using Gallery.ConfigFiles;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.ToiletNpc
{
	public class ToiletNpcController : BaseController
	{
		public SexPlace.SexPlaceType PlaceType = SexPlace.SexPlaceType.Toilet;

		public int Grade;

		public override void Unlock(GalleryChara[] charas)
		{
			if (charas.Length < 2)
			{
				PLogger.LogError($"ToiletNpcController: Not enough actors. Expected 2, got {charas.Length}");
				return;
			}

			var desc = $"{charas[0]} x {charas[1]} (Grade: {this.Grade}, Place Type: {this.PlaceType})";

			var actors = new GalleryActor[2] { new(charas[0]), new(charas[1]) };
			if (this.IsUnlocked(actors))
			{
				PLogger.LogInfo($"ToiletNpcController: Already unlocked: {desc}");
				return;
			}

			PLogger.LogInfo($"ToiletNpcController: Unlocking: {desc}");
			SaveFile.GalleryState.Instance.ToiletNpc.Add(
				new ToiletNPCInteraction(charas[0], charas[1], this.PlaceType, this.Grade)
			);
		}

		public override bool IsUnlocked(GalleryActor[] actors)
		{
			if (actors.Length < 2)
			{
				PLogger.LogError($"ToiletNpcController: Not enough actors. Expected 2, got {actors.Length}");
				return false;
			}

			return GalleryState.Instance.ToiletNpc.Any((interaction) =>
			{
				return interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character2.Id == actors[1].NpcId
					&& interaction.PlaceType == this.PlaceType
					&& interaction.ToiletGrade == this.Grade
					;
			});
		}

		protected override IEnumerator GetScene(PlayData playData)
		{
			if (playData.Actors.Count < 2)
			{
				PLogger.LogError($"ToiletNpcController: Not enough actors. Expected 2, got {playData.Actors.Count}");
				yield break;
			}

			// @TODO: Implement it once HFramework supports it.
			throw new NotImplementedException();
		}
	}
}

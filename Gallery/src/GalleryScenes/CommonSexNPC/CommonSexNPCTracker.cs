using YotanModCore;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.CommonSexNPC
{
	public class CommonSexNPCTracker : BaseTracker
	{
		private readonly GalleryChara NpcA;

		private readonly GalleryChara NpcB;

		private readonly int PlaceGrade;

		private readonly SexPlace.SexPlaceType PlaceType;

		private readonly SexManager.SexCountState SexType;


		public CommonSexNPCTracker(
			CommonStates npcA,
			CommonStates npcB,
			SexPlace sexPlace,
			SexManager.SexCountState sexType
		) : base()
		{
			// @TODO: Revert this so male comes first
			if (CommonUtils.IsFemale(npcA) && !CommonUtils.IsFemale(npcB))
			{
				CommonStates temp = npcA;
				npcA = npcB;
				npcB = temp;
			}

			this.NpcA = new GalleryChara(npcA);
			this.NpcB = new GalleryChara(npcB);

			this.PlaceGrade = sexPlace?.grade ?? -1;
			this.PlaceType = sexPlace?.placeType ?? SexPlace.SexPlaceType.Normal;
			this.SexType = sexType;
		}

		public override void End()
		{
			// Same sex (based on being same npc type... I guess) won't have creampie, but it is a success anyway. Creampie is always success
			if (this.NpcA.Id == this.NpcB.Id || this.DidCreampie)
			{
				new CommonSexNPCController() {
					PlaceGrade = this.PlaceGrade,
					PlaceType = this.PlaceType,
					SexType = this.SexType
				}.Unlock([this.NpcA, this.NpcB]);
			}
			else
			{
				var desc = $"{this.NpcA} x {this.NpcB} (Grade: {this.PlaceGrade}, Place Type: {this.PlaceType}, Sex Type: {this.SexType})";
				GalleryLogger.LogDebug($"CommonSexNPCSceneTracker#OnEnd: 'DidCreampie' not set -- event NOT unlocked for {desc}");
			}
		}
	}
}

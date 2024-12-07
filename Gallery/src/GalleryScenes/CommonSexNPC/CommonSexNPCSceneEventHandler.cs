using YotanModCore;
using Gallery.SaveFile.Containers;
using ExtendedHSystem;
using System.Collections;
using ExtendedHSystem.Scenes;

namespace Gallery.GalleryScenes.CommonSexNPC
{
	public class CommonSexNPCSceneEventHandler : SceneEventHandler
	{
		/// <summary>
		/// First NPC in the Sex Scene.
		/// If a female NPC is involved, this is the female one.
		/// </summary>
		private readonly GalleryChara NpcA;

		private readonly GalleryChara NpcB;

		private readonly int PlaceGrade;

		private readonly SexPlace.SexPlaceType PlaceType;

		private readonly SexManager.SexCountState SexType;

		private bool DidCreampie;

		public CommonSexNPCSceneEventHandler(
			CommonStates npcA,
			CommonStates npcB,
			SexPlace sexPlace,
			SexManager.SexCountState sexType
		) : base("yogallery_commonsexnpc_handler")
		{
			if (!CommonUtils.IsFemale(npcA) && CommonUtils.IsFemale(npcB))
			{
				CommonStates temp = npcA;
				npcA = npcB;
				npcB = temp;
			}

			this.NpcA = new GalleryChara(npcA);
			this.NpcB = new GalleryChara(npcB);

			this.PlaceGrade = sexPlace.grade;
			this.PlaceType = sexPlace.placeType;
			this.SexType = sexType;
		}

		private CommonStates GetFriend(CommonStates npcA, CommonStates npcB)
		{
			if (CommonUtils.IsFriend(npcA))
				return npcA;

			if (CommonUtils.IsFriend(npcB))
				return npcB;

			return null;
		}

		public override IEnumerable OnCreampie(CommonStates from, CommonStates to)
		{
			this.DidCreampie = true;
			return base.OnCreampie(from, to);
		}

		public override IEnumerable AfterSex(IScene scene, CommonStates from, CommonStates to)
		{
			// Same sex (based on being same npc type... I guess) won't have creampie, but it is a success anyway. Creampie is always success
			if (from.npcID == to.npcID || this.DidCreampie) {
				CommonSexNPCSceneManager.Instance.Unlock(this.NpcA, this.NpcB, this.PlaceGrade, this.PlaceType, this.SexType);
			} else {
				var desc = $"{this.NpcA} x {this.NpcB} (Grade: {this.PlaceGrade}, Place Type: {this.PlaceType}, Sex Type: {this.SexType})";
				GalleryLogger.LogDebug($"CommonSexNPCSceneTracker#OnEnd: 'DidCreampie' not set -- event NOT unlocked for {desc}");
			}

			return base.AfterSex(scene, from, to);
		}
	}
}
using System.Collections.Generic;
using YotanModCore;
using Gallery.Patches;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.CommonSexNPC
{
	public class CommonSexNPCSceneTracker
	{
		private class RunningScene
		{
			public GalleryChara NpcA;

			public GalleryChara NpcB;

			public int PlaceGrade;

			public SexPlace.SexPlaceType PlaceType;

			public SexManager.SexCountState SexType;

			public bool DidCreampie;
		}

		public delegate void UnlockInfo(GalleryChara npcA, GalleryChara npcB, int placeGrade, SexPlace.SexPlaceType placeType, SexManager.SexCountState sexType);

		public event UnlockInfo OnUnlock;

		// friendId => Scene (Friend ID from NPC A, then NPC B, first comes, first pick)
		private readonly Dictionary<int, RunningScene> runningScenes = new Dictionary<int, RunningScene>();

		public CommonSexNPCSceneTracker()
		{
			CommonSexNPCPatch.OnStart += this.OnStart;
			CommonSexNPCPatch.OnEnd += this.OnEnd;
			SexCountPatch.OnCreampie += this.OnCreampieCount;
		}

		private CommonStates GetFriend(CommonStates npcA, CommonStates npcB)
		{
			if (CommonUtils.IsFriend(npcA))
				return npcA;

			if (CommonUtils.IsFriend(npcB))
				return npcB;

			return null;
		}

		private RunningScene TryGetScene(CommonStates npcA, CommonStates npcB, out int friendId)
		{
			if (CommonUtils.IsFriend(npcA) && this.runningScenes.TryGetValue(npcA.friendID, out RunningScene sceneA)) {
				friendId = npcA.friendID;
				return sceneA;
			}

			if (CommonUtils.IsFriend(npcB) && this.runningScenes.TryGetValue(npcB.friendID, out RunningScene sceneB)) {
				friendId = npcA.friendID;
				return sceneB;
			}

			friendId = -1;
			return null;
		}

		private void OnStart(CommonSexNPCPatch.CommonSexNpcInfo info)
		{
			var friend = GetFriend(info.NpcA, info.NpcB);
			if (friend == null) {
				GalleryLogger.LogDebug($"CommonSexNPCSceneTracker#OnStart: no friends between {info.NpcA.charaName} and {info.NpcB.charaName}");
				return;
			}

			RunningScene scene;
			if (this.runningScenes.TryGetValue(friend.friendID, out scene)) {
				GalleryLogger.LogError($"CommonSexNPCSceneTracker#OnStart: scene already running for {friend.charaName}. Resetting");

				this.runningScenes.Remove(friend.friendID);
			}

			scene = new RunningScene()
			{
				NpcA = new GalleryChara(info.NpcA),
				NpcB = new GalleryChara(info.NpcB),
				PlaceGrade = info.SexPlace?.grade ?? -1,
				PlaceType = info.SexPlace?.placeType ?? SexPlace.SexPlaceType.Normal,
				SexType = info.SexType,
				DidCreampie = false,
			};
			PLogger.LogInfo($">> Storing for {friend.charaName} / {friend.friendID}");
			this.runningScenes.Add(friend.friendID, scene);
		}

		private void OnCreampieCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			int friendId;
			var scene = this.TryGetScene(value.from, value.to, out friendId);
			if (scene == null) {
				// They may be in other scene
				// LoveLogger.LogError($"CommonSexNPCSceneTracker#OnEnd: no scene between {info.NpcA.charaName} and {info.NpcB.charaName}");
				return;
			}

			scene.DidCreampie = true;
		}

		private void OnEnd(CommonSexNPCPatch.CommonSexNpcInfo info)
		{
			if (info.NpcA == null || info.NpcB == null) {
				GalleryLogger.LogError($"CommonSexNPCSceneTracker#OnEnd: chara is null ({info.NpcA}, {info.NpcB})");
				return;
			}

			int friendId;
			var scene = this.TryGetScene(info.NpcA, info.NpcB, out friendId);
			if (scene == null) {
				// They may be in other scene
				// LoveLogger.LogError($"CommonSexNPCSceneTracker#OnEnd: no scene between {info.NpcA.charaName} and {info.NpcB.charaName}");
				return;
			}

			this.runningScenes.Remove(friendId);
			
			// Same sex (based on being same npc type... I guess) won't have creampie, but it is a success anyway. Creampie is always success
			if (scene.NpcA.Id == scene.NpcB.Id || scene.DidCreampie) {
				this.OnUnlock?.Invoke(scene.NpcA, scene.NpcB, scene.PlaceGrade, scene.PlaceType, scene.SexType);
			} else {
				var desc = $"{scene.NpcA} x {scene.NpcB} (Grade: {scene.PlaceGrade}, Place Type: {scene.PlaceType}, Sex Type: {scene.SexType})";
				GalleryLogger.LogDebug($"CommonSexNPCSceneTracker#OnEnd: 'DidCreampie' not set -- event NOT unlocked for {desc}");
			}
		}
	}
}
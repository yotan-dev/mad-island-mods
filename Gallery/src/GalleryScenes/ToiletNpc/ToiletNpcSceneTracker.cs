using System.Collections.Generic;
using YotanModCore;
using Gallery.Patches;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.ToiletNpc
{
	public class ToiletNpcSceneTracker
	{
		private struct RunningScene
		{
			public GalleryChara User;

			public GalleryChara Target;

			public int PlaceGrade;

			public SexPlace.SexPlaceType PlaceType;

			public bool DidToilet;

			public bool DidCreampie;
		}

		public delegate void UnlockInfo(GalleryChara user, GalleryChara target, int placeGrade, SexPlace.SexPlaceType placeType);

		public event UnlockInfo OnUnlock;

		// friendId => Scene (Friend ID from NPC A, then NPC B, first comes, first pick)
		private readonly Dictionary<int, RunningScene> runningScenes = new Dictionary<int, RunningScene>();

		public ToiletNpcSceneTracker()
		{
			ToiletNpcPatch.OnStart += this.OnStart;
			ToiletNpcPatch.OnEnd += this.OnEnd;
			SexCountPatch.OnCreampie += this.OnCreampieCount;
			SexCountPatch.OnToilet += this.OnToiletCount;
		}

		private void OnStart(ToiletNpcPatch.ToiletNpcInfo info)
		{
			if (!CommonUtils.IsFriend(info.User)) {
				GalleryLogger.LogError($"ToiletNpcSceneTracker#OnStart: Non-friend NPC using toilet. Unhandled case... {info.User.charaName}");
				return;
			}

			RunningScene scene;
			if (this.runningScenes.TryGetValue(info.User.friendID, out scene)) {
				GalleryLogger.LogError($"ToiletNpcSceneTracker#OnStart: scene already running for {info.User.charaName}. Resetting");

				this.runningScenes.Remove(info.User.friendID);
			}

			scene = new RunningScene()
			{
				User = new GalleryChara(info.User),
				Target = new GalleryChara(info.Target),
				PlaceGrade = info.SexPlace.grade,
				PlaceType = info.SexPlace.placeType,
				DidToilet = false,
				DidCreampie = false,
			};
			this.runningScenes.Add(info.User.friendID, scene);
		}

		private void OnToiletCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			var friendId = value.from.friendID;
			RunningScene scene;
			if (!this.runningScenes.TryGetValue(friendId, out scene)) {
				// They may be in other scene
				// LoveLogger.LogDebug($"ToiletNpcSceneTracker#OnCreampieCount: no scene between {value.from.charaName} and {value.to.charaName}");
				return;
			}

			scene.DidToilet = true;
		}

		private void OnCreampieCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			var friendId = value.from.friendID;
			RunningScene scene;
			if (!this.runningScenes.TryGetValue(friendId, out scene)) {
				// They may be in other scene
				// LoveLogger.LogDebug($"ToiletNpcSceneTracker#OnCreampieCount: no scene between {value.from.charaName} and {value.to.charaName}");
				return;
			}

			scene.DidCreampie = true;
		}

		private void OnEnd(ToiletNpcPatch.ToiletNpcInfo info)
		{
			if (info.User == null || info.Target == null) {
				GalleryLogger.LogError($"ToiletNpcSceneTracker#OnEnd: chara is null ({info.User}, {info.Target})");
				return;
			}

			RunningScene scene;
			if (!this.runningScenes.TryGetValue(info.User.friendID, out scene)) {
				// They may be in other scene
				// LoveLogger.LogError($"ToiletNpcSceneTracker#OnEnd: no scene between {info.NpcA.charaName} and {info.NpcB.charaName}");
				return;
			}

			this.runningScenes.Remove(info.User.friendID);
			
			if (scene.DidToilet && scene.DidCreampie) {
				this.OnUnlock?.Invoke(scene.User, scene.Target, scene.PlaceGrade, scene.PlaceType);
			} else {
				var desc = $"{scene.User} x {scene.Target} (Grade: {scene.PlaceGrade}, Place Type: {scene.PlaceType})";
				GalleryLogger.LogDebug($"ToiletNpcSceneTracker#OnEnd: 'DidCreampie'/'DidToilet' not set -- event NOT unlocked for {desc}");
			}
		}
	}
}
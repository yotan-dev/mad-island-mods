using System;
using System.Collections.Generic;
using YotanModCore;
using Gallery.Patches;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Delivery
{
	public class DeliverySceneTracker
	{
		public delegate void UnlockInfo(GalleryChara girl);

		public event UnlockInfo OnUnlock;

		private class SceneContainer {
			public GalleryChara Girl = null;

			public bool DidDelivery = false;
		}

		// friendID => Scene
		private Dictionary<int, SceneContainer> scenes = new Dictionary<int, SceneContainer>();

		public DeliverySceneTracker()
		{
			DeliveryPatch.OnStart += this.OnStart;
			DeliveryPatch.OnEnd += this.OnEnd;
			SexCountPatch.OnDelivery += this.OnDelivery;
		}

		private void OnStart(CommonStates girl)
		{
			GalleryLogger.LogDebug($"DeliverySceneTracker#OnStart");
			
			if (!CommonUtils.IsFriend(girl))
				return;

			var friendId = girl.friendID;
			if (this.scenes.ContainsKey(friendId)) {
				GalleryLogger.LogError($"DeliverySceneTracker#OnStart: scene already running for {girl.charaName}. Resetting.");
				this.scenes.Remove(friendId);
			}

			this.scenes.Add(friendId, new SceneContainer() { Girl = new GalleryChara(girl) });
		}

		private void OnDelivery(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (!this.scenes.TryGetValue(value.to.friendID, out var scene)) {
				return;
			}

			scene.DidDelivery = true;
		}

		private void OnEnd(CommonStates girl)
		{
			if (!this.scenes.TryGetValue(girl.friendID, out var scene)) {
				return;
			}

			if (scene.DidDelivery) {
				this.OnUnlock?.Invoke(scene.Girl);
			} else {
				GalleryLogger.LogDebug($"DeliveryScene: OnEnd: 'DeliveryCounted' not set -- event NOT unlocked for {girl}");
			}
		}
	}
}
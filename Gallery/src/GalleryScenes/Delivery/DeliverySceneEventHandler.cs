using Gallery.SaveFile.Containers;
using HFramework;
using System.Collections;
using HFramework.Scenes;

namespace Gallery.GalleryScenes.Delivery
{
	public class DeliverySceneEventHandler : SceneEventHandler
	{
		private readonly GalleryChara Mother;

		private bool DidDelivery = false;

		public DeliverySceneEventHandler(CommonStates mother)
			: base("yogallery_delivery_handler")
		{
			this.Mother = new GalleryChara(mother);
		}

		public override IEnumerable OnDelivery(HFramework.Scenes.Delivery scene, CommonStates mother)
		{
			this.DidDelivery = true;
			yield return null;
		}

		public override IEnumerable AfterDelivery(IScene scene, CommonStates mother)
		{
			if (this.DidDelivery)
				DeliverySceneManager.Instance.Unlock(this.Mother);
			else
				GalleryLogger.LogDebug($"DeliverySceneEventHandler: OnEnd: 'DidDelivery' not set -- event NOT unlocked for {this.Mother}");

			yield return null;
		}
	}
}

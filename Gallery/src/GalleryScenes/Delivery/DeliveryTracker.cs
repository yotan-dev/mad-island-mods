using Gallery.GalleryScenes.CommonSexNPC;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Delivery
{
	public class DeliveryTracker : BaseTracker
	{
		private readonly GalleryChara Mother;

		public DeliveryTracker(CommonStates mother) : base()
		{
			this.Mother = new GalleryChara(mother);
		}

		public override void End()
		{
			if (this.DidDelivery)
				new DeliveryController().Unlock([this.Mother]);
			else
				GalleryLogger.LogDebug($"DeliverySceneEventHandler: OnEnd: 'DidDelivery' not set -- event NOT unlocked for {this.Mother}");
		}
	}
}

namespace Gallery.GalleryScenes
{
	public abstract class BaseTracker
	{
		public virtual bool DidNormal { get; set; } = false;

		public virtual bool DidCreampie { get; set; } = false;

		public virtual bool DidDelivery { get; set; } = false;

		public virtual bool DidToilet { get; set; }= false;

		public virtual bool Raped { get; set; } = false;

		public virtual bool Pregnant { get; set; } = false;

		public virtual bool Busted { get; set; } = false;

		public abstract void End();
	}
}

namespace Gallery.GalleryScenes
{
	public abstract class BaseTracker
	{
		// Make all properties virtual so child classes can override the get/sets to do something more
		// E.g. ManSleepRapeTracker overrides the get/set to set for a mode

		public virtual bool DidNormal { get; set; } = false;

		public virtual bool DidCreampie { get; set; } = false;

		public virtual bool DidDelivery { get; set; } = false;

		public virtual bool DidToilet { get; set; }= false;

		public virtual bool DidMasturbation { get; set; } = false;

		public virtual bool Raped { get; set; } = false;

		public virtual bool Pregnant { get; set; } = false;

		public virtual bool Busted { get; set; } = false;

		public virtual string PerformerId { get; set; } = "";

		public abstract void End();
	}
}

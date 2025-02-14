using Gallery.GalleryScenes.CommonSexNPC;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Onani
{
	public class OnaniTracker : BaseTracker
	{
		public GalleryChara Npc;

		private bool Perfume;

		public OnaniTracker(CommonStates npc) : base()
		{
			this.Npc = new GalleryChara(npc);
			this.Perfume = npc.debuff.perfume > 0f;
		}

		public override void End()
		{
			if (!this.DidMasturbation)
			{
				var desc = $"{this.Npc} (Perfume: {this.Perfume})";
				GalleryLogger.LogDebug($"OnaniTracker: Masturbation not set -- event NOT unlocked for {desc}");
				return;
			}

			new OnaniController() { Perfume = this.Perfume }.Unlock(this.PerformerId, [this.Npc]);
		}

		public void LoadPerformerId()
		{
			this.PerformerId = GalleryScenesManager.Instance.FindPerformer(typeof(OnaniController), [this.Npc]);
		}
	}
}

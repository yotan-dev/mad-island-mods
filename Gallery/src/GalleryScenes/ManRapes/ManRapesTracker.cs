using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.ManRapes
{
	public class ManRapesTracker : BaseTracker
	{
		private readonly GalleryChara Man;
		private readonly GalleryChara Girl;

		public ManRapesTracker(CommonStates man, CommonStates girl) : base() {
			this.Man = new GalleryChara(man);
			this.Girl = new GalleryChara(girl);
		}

		public override void End()
		{
			if (this.Raped && this.DidCreampie) {
				ManRapesSceneManager.Instance.Unlock(this.Man, this.Girl);
			} else {
				GalleryLogger.LogDebug($"ManRapeScene: OnEnd: 'Raped' or 'DidCreampie' not set -- event NOT unlocked for {Girl}");
			}
		}
	}
}

using Gallery.SaveFile.Containers;
using HFramework;
using System.Collections;
using HFramework.Scenes;

namespace Gallery.GalleryScenes.ManRapes
{
	public class ManRapesSceneEventHandler : SceneEventHandler
	{
		private readonly GalleryChara Man;
		private readonly GalleryChara Girl;

		private bool DidRape = false;

		private bool DidCreampie = false;

		public ManRapesSceneEventHandler(CommonStates man, CommonStates girl) : base("yogallery_manrapes_handler") {
			this.Man = new GalleryChara(man);
			this.Girl = new GalleryChara(girl);
		}

		public override IEnumerable OnCreampie(CommonStates from, CommonStates to)
		{
			this.DidCreampie = true;
			yield return null;
		}

		public override IEnumerable OnRape(IScene scene, CommonStates from, CommonStates to)
		{
			this.DidRape = true;
			yield return null;
		}

		public override IEnumerable AfterManRape(CommonStates victim, CommonStates rapist)
		{
			if (this.DidRape && this.DidCreampie) {
				ManRapesSceneManager.Instance.Unlock(this.Man, this.Girl);
			} else {
				GalleryLogger.LogDebug($"ManRapeScene: OnEnd: 'DidRape' or 'DidCreampie' not set -- event NOT unlocked for {Girl}");
			}

			yield return null;
		}
	}
}

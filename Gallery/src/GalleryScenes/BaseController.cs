using System.Collections;
using System.Xml.Serialization;
using Gallery.ConfigFiles;
using Gallery.GalleryScenes.AssWall;
using Gallery.GalleryScenes.CommonSexNPC;
using Gallery.GalleryScenes.CommonSexPlayer;
using Gallery.GalleryScenes.Daruma;
using Gallery.GalleryScenes.Delivery;
using Gallery.GalleryScenes.ManRapes;
using Gallery.GalleryScenes.ManSleepRape;
using Gallery.GalleryScenes.Onani;
using Gallery.GalleryScenes.PlayerRaped;
using Gallery.GalleryScenes.Slave;
using Gallery.GalleryScenes.Toilet;
using Gallery.SaveFile.Containers;
using HFramework.Scenes;

namespace Gallery.GalleryScenes
{
	[XmlInclude(typeof(AssWallController))]
	[XmlInclude(typeof(CommonSexNPCController))]
	[XmlInclude(typeof(CommonSexPlayerController))]
	[XmlInclude(typeof(DarumaController))]
	[XmlInclude(typeof(DeliveryController))]
	[XmlInclude(typeof(ManRapesController))]
	[XmlInclude(typeof(ManSleepRapeController))]
	[XmlInclude(typeof(OnaniController))]
	[XmlInclude(typeof(PlayerRapedController))]
	[XmlInclude(typeof(SlaveController))]
	[XmlInclude(typeof(ToiletController))]
	public abstract class BaseController
	{
		protected IScene Scene { get; set; }

		public string PerformerId { get; set; }

		public string Prop { get; set; }

		public abstract void Unlock(string performerId, GalleryChara[] charas);

		public abstract bool IsUnlocked(GalleryActor[] actors);
		
		public abstract bool IsUnlocked(string performerId, GalleryActor[] actors);

		protected abstract IEnumerator GetScene(PlayData playData);

		protected bool EnsurePerformer(string performerId)
		{
			if (!GalleryScenesManager.Instance.HasPerformerForController(this.GetType(), performerId))
			{
				PLogger.LogInfo($"Skipping because {performerId} does not exists for {this.GetType()}");
				return false;
			}

			return true;
		}

		public IEnumerator Play(PlayData playData)
		{
			if (this.Scene != null)
				this.Destroy();

			yield return this.GetScene(playData);

			if (this.Scene == null)
			{
				PLogger.LogError(">> Scene not found");
				yield break;
			}

			this.Scene.SetController(new GallerySceneController());
			yield return null;
			yield return this.Scene.Run();
			yield return null; // Give some time for any clean up;

			this.Scene = null;
		}

		public void Destroy()
		{
			if (this.Scene != null)
				this.Scene.Destroy();

			this.Scene = null;
		}
	}
}

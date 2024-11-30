using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YotanModCore;

namespace ExtendedHSystem
{
	public class GallerySceneEventHandler : SceneEventHandler
	{
		public GallerySceneEventHandler() : base("exthsystem_gallery_handler")
		{
		}

		public override IEnumerable PlayerDefeated()
		{
			Managers.mn.uiMN.skip = false;
			yield return null;
		}

		public override IEnumerable BeforeRespawn()
		{
			Managers.mn.uiMN.SkipView(false);
			yield return null;
		}

		public override IEnumerable Respawn(CommonStates player, CommonStates other)
		{
			yield break;
		}
	}
}
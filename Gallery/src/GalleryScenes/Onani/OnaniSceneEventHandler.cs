using System.Collections;
using HFramework;
using HFramework.Scenes;
using Gallery.SaveFile.Containers;
using YotanModCore.Consts;

namespace Gallery.GalleryScenes.Onani
{
	public class OnaniSceneEventHandler : SceneEventHandler
	{
		public GalleryChara Npc;

		private bool Perfume;

		private int MasturbateCount;

		public OnaniSceneEventHandler(CommonStates npc) : base("yogallery_onani_handler")
		{
			this.Npc = new GalleryChara(npc);
			this.MasturbateCount = this.GetMasturbationCount(npc);
			this.Perfume = npc.debuff.perfume > 0f;
		}

		private int GetMasturbationCount(CommonStates npc)
		{
			return npc.sexInfo.Length > SexInfoIndex.Masturbation ? npc.sexInfo[SexInfoIndex.Masturbation] : 0;
		}

		public override IEnumerable AfterMasturbate(IScene scene, CommonStates common)
		{
			if (this.GetMasturbationCount(common) <= this.MasturbateCount)
			{
				var desc = $"{this.Npc} (Perfume: {this.Perfume})";
				GalleryLogger.LogDebug($"OnaniSceneEventHandler#AfterMasturbate: Count did not increase ({this.MasturbateCount} -> {this.GetMasturbationCount(common)}) -- event NOT unlocked for {desc}");
				yield break;
			}

			OnaniSceneManager.Instance.Unlock(this.Npc, this.Perfume);
		}
	}
}

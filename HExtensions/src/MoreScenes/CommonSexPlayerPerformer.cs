using HFramework.Performer;
using HFramework.Scenes;
using UnityEngine;

namespace HExtensions.MoreScenes
{
	public class CommonSexPlayerPerformer : CommonSexPlayer
	{
		private string SceneId;

		private string PerformerId;

		public CommonSexPlayerPerformer(string sceneId, string performerId, CommonStates playerCommon, CommonStates npcCommon, Vector3 pos, int sexType)
			: base(playerCommon, npcCommon, pos, sexType)
		{
			this.SceneId = sceneId;
			this.PerformerId = performerId;
		}

		protected override SexPerformer SelectPerformer()
		{
			return ScenesManager.Instance.GetPerformer(this.SceneId, this.PerformerId, this.Controller);
		}

		public override string GetName()
		{
			return "HExt_CommonSexPlayerPerformer";
		}
	}
}

#nullable enable

using Spine.Unity;
using UnityEngine;

namespace HFramework.Tree
{
	public interface ISexScriptMenuSession
	{
		void SetOptions((string Id, string Text)[] options);
		void Show();
		void Hide();
		void Close();
	}

	public class ContextNpc
	{
		public CommonStates Common { get; set; }
		public float? Angle { get; set; }

		public ContextNpc(CommonStates common, float? angle)
		{
			Common = common;
			Angle = angle;
		}
	}

	public class CommonContext
	{
		public ContextNpc[] Npcs { get; set; } = [];

		public ISexScriptMenuSession? MenuSession { get; set; }

		public string? PendingChoiceId { get; set; }

		public string? LastChoiceId { get; set; }

		public SexPlace? SexPlace { get; set; }

		public Vector3? SexPlacePos { get; set; }

		public GameObject? TmpSex { get; set; }

		public SkeletonAnimation? TmpSexAnim { get; set; }

		public int SexType = -1;

		public bool TryConsumeChoice(out string choiceId)
		{
			choiceId = this.PendingChoiceId ?? "";
			if (this.PendingChoiceId == null)
				return false;

			this.PendingChoiceId = null;
			this.LastChoiceId = choiceId;
			return true;
		}
	}
}

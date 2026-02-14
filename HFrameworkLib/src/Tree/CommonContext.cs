#nullable enable

using System;
using Spine.Unity;
using UnityEngine;

namespace HFramework.Tree
{
	[Serializable]
	public enum SexAction
	{
		Idle,
		Caressing,
		SexSlow,
		SexFast,
		Cumming,
	}

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
		public ContextNpc[] Actors { get; set; } = [];

		public ISexScriptMenuSession? MenuSession { get; set; }

		public SexAction SexAction { get; set; } = SexAction.Idle;

		public string? PendingChoiceId { get; set; }

		public string? LastChoiceId { get; set; }

		public SexPlace? SexPlace { get; set; }

		public Vector3? SexPlacePos { get; set; }

		public GameObject? TmpSex { get; set; }

		public SkeletonAnimation? TmpSexAnim { get; set; }

		/// <summary>
		/// Whether the main canvas visibility has been changed by a node.
		/// We use that to restore the canvas on Teardown
		/// Note: We only expect to have 1 script touching the canvas at any given time
		/// </summary>
		public bool HasChangedMainCanvasVisibility { get; set; }

		/// <summary>
		/// Whether the sex meter has been created by a node.
		/// We use that to hide the sex meter on Teardown.
		/// Note: We only expect to have 1 script using sex meter at any given time
		/// </summary>
		public bool HasSexMeter { get; set; }

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

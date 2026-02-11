#nullable enable

using Spine.Unity;
using UnityEngine;

namespace HFramework.Tree
{
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

		public SexPlace? SexPlace { get; set; }

		public Vector3? SexPlacePos { get; set; }

		public GameObject? TmpSex { get; set; }

		public SkeletonAnimation? TmpSexAnim { get; set; }

		public int SexType = -1;
	}
}

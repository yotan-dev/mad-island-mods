#nullable enable

using Spine.Unity;
using UnityEngine;

namespace HFramework.Tree
{
	public class CommonContext
	{
		/// <summary>
		/// First NPC in the sex scene.
		/// If a Male x Female relation, this is the Male.
		/// </summary>
		public CommonStates? NpcA { get; set; }

		/// <summary>
		/// Second NPC in the sex scene.
		/// If a Male x Female relation, this is the Female.
		/// </summary>
		public CommonStates? NpcB { get; set; }

		public SexPlace? SexPlace { get; set; }

		public Vector3? SexPlacePos { get; set; }

		public float? NpcAAngle { get; set; } = null;

		public float? NpcBAngle { get; set; } = null;

		public GameObject? TmpSex { get; set; }

		public SkeletonAnimation? TmpSexAnim { get; set; }

		public CommonContext Clone()
		{
			// @TODO: Maybe create instead of clone
			return new CommonContext
			{
				NpcA = NpcA,
				NpcB = NpcB,
				SexPlace = SexPlace,
				SexPlacePos = SexPlacePos,
				NpcAAngle = NpcAAngle,
				NpcBAngle = NpcBAngle,
				TmpSex = TmpSex,
				TmpSexAnim = TmpSexAnim
			};
		}
	}
}

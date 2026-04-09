using System.Collections;
using HFramework.Scenes;
using Spine.Unity;
using UnityEngine;

namespace HFramework.Handlers
{
	/// <summary>
	/// Sets animation "name" to loop indefinetely in "animation"
	/// </summary>
	public class PlayerGrappled : BaseHandler
	{
		private readonly SkeletonAnimation Anim;

		private readonly CommonStates Player;

		public PlayerGrappled(IScene scene, SkeletonAnimation animation, CommonStates player) : base(scene)
		{
			this.Anim = animation;
			this.Player = player;
		}

		protected override IEnumerator Run()
		{
			PlayerMove pMove = this.Player.pMove;
			if (pMove == null)
			{
				this.ShouldStop = true;
				yield break;
			}

			float faintTime = 1f;
			while (this.Scene.CanContinue() && this.Player.faint > 0.0 && pMove.common.dead != 0)
			{
				faintTime -= Time.deltaTime;
				if (faintTime <= 0f)
				{
					faintTime = 1f;
					this.Player.FaintChange(-5.0, true, true);
				}
				yield return null;
			}
		}
	}
}

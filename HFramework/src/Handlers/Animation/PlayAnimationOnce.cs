using System.Collections;
using HFramework.Scenes;
using Spine.Unity;
using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.Handlers.Animation
{
	/// <summary>
	/// Plays animation "name" in "animation" once.
	/// - Stops the scene if the animation can't be played until the end
	/// - If "skipable" = true, the animation can be skipped by clicking the mouse
	/// </summary>
	public class PlayAnimationOnce : BaseHandler
	{
		private readonly SkeletonAnimation Anim;

		private readonly string Name;

		private readonly bool Skipable;

		public PlayAnimationOnce(
			IScene scene,
			SkeletonAnimation animation,
			string name,
			bool skipable = false
		) : base(scene)
		{
			this.Anim = animation;
			this.Name = name;
			this.Skipable = skipable;
		}

		protected override IEnumerator Run()
		{
			float animTime = 0f;
			if (this.Anim.HasAnimation(this.Name))
			{
				this.Anim.state.SetAnimation(0, this.Name, false);
				animTime = this.Anim.state.GetCurrent(0).AnimationEnd;
			}

			while (animTime >= 0f && this.Scene.CanContinue())
			{
				if (this.Skipable && Input.GetMouseButtonDown(0))
					yield break;

				animTime -= Time.deltaTime;
				yield return false;
			}

			if (animTime > 0f)
				this.ShouldStop = true;
		}
	}
}

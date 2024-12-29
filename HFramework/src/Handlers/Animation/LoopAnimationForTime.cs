using System.Collections;
using HFramework.Scenes;
using Spine.Unity;
using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.Handlers.Animation
{
	public class LoopAnimationForTime : BaseHandler
	{
		private readonly SkeletonAnimation Anim;

		private readonly string Name;

		private readonly float Duration;

		public LoopAnimationForTime(IScene scene, SkeletonAnimation animation, string name, float duration) : base(scene)
		{
			this.Anim = animation;
			this.Name = name;
			this.Duration = duration;
		}

		protected override IEnumerator Run()
		{
			if (this.Anim.HasAnimation(this.Name))
				this.Anim.state.SetAnimation(0, this.Name, true);

			float animTime = this.Duration;
			while (animTime >= 0f && this.Scene.CanContinue())
			{
				animTime -= Time.deltaTime;
				yield return false;
			}

			if (animTime > 0f)
				this.ShouldStop = true;
		}
	}
}

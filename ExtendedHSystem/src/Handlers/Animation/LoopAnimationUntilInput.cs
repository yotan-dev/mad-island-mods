using System.Collections;
using ExtendedHSystem.Scenes;
using Spine.Unity;
using UnityEngine;
using YotanModCore.Extensions;

namespace ExtendedHSystem.Handlers.Animation
{
	public class LoopAnimationUntilInput : BaseHandler
	{
		private readonly SkeletonAnimation Anim;

		private readonly string Name;

		public LoopAnimationUntilInput(IScene scene, SkeletonAnimation animation, string name) : base(scene)
		{
			this.Anim = animation;
			this.Name = name;
		}

		protected override IEnumerator Run()
		{
			if (this.Anim.HasAnimation(this.Name))
				this.Anim.state.SetAnimation(0, this.Name, true);

			yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
		}
	}
}
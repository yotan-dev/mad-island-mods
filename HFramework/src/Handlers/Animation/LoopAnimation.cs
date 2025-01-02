using System.Collections;
using HFramework.Scenes;
using Spine.Unity;
using YotanModCore.Extensions;

namespace HFramework.Handlers.Animation
{
	/// <summary>
	/// Sets animation "name" to loop indefinetely in "animation"
	/// </summary>
	public class LoopAnimation : BaseHandler
	{
		private readonly SkeletonAnimation Anim;

		private readonly string Name;

		public LoopAnimation(IScene2 scene, SkeletonAnimation animation, string name) : base(scene)
		{
			this.Anim = animation;
			this.Name = name;
		}

		public LoopAnimation(IScene oldScene, SkeletonAnimation animation, string name) : base(oldScene)
		{
			this.Anim = animation;
			this.Name = name;
		}

		protected override IEnumerator Run()
		{
			if (this.Anim.HasAnimation(this.Name))
				this.Anim.state.SetAnimation(0, this.Name, true);

			yield break;
		}
	}
}

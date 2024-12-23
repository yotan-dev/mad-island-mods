using System.Collections;
using ExtendedHSystem.Scenes;
using Spine.Unity;
using YotanModCore.Extensions;

namespace ExtendedHSystem.Handlers.Animation
{
	/// <summary>
	/// Sets animation "name" to loop indefinetely in "animation"
	/// </summary>
	public class LoopAnimation : BaseHandler
	{
		private readonly SkeletonAnimation Anim;

		private readonly string Name;

		public LoopAnimation(IScene scene, SkeletonAnimation animation, string name) : base(scene)
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
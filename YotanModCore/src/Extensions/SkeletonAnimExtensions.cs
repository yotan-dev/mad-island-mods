using Spine.Unity;

namespace YotanModCore.Extensions
{
	public static class SkeletonAnimExtensions
	{
		/// <summary>
		/// Returns whether the SkeletonAnimation has an animation with the given name
		/// </summary>
		/// <param name="anim"></param>
		/// <param name="animationName"></param>
		/// <returns></returns>
		public static bool HasAnimation(this SkeletonAnimation anim, string animationName)
		{
			return anim.skeleton.Data.FindAnimation(animationName) != null;
		}

		/// <summary>
		/// Get the current animation name.
		/// Note: It assumes trackIndex 0
		/// </summary>
		/// <param name="anim"></param>
		/// <returns></returns>
		public static string GetCurrentAnimName(this SkeletonAnimation anim) {
			return anim.state.GetCurrent(0).Animation.Name;
		}
	}

}

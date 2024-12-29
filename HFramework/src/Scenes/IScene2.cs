using System.Collections;
using HFramework.Performer;
using Spine.Unity;

namespace HFramework.Scenes
{
	// @TODO: Merge into IScene once API is nice
	public interface IScene2 : IScene
	{
		string GetName();
		CommonStates[] GetActors();
		SkeletonAnimation GetSkelAnimation();
		string ExpandAnimationName(string originalName);
		SexPerformer GetPerformer();
	}
}

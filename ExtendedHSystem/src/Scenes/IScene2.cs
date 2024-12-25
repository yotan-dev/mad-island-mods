using System.Collections;
using Spine.Unity;

namespace ExtendedHSystem.Scenes
{
	// @TODO: Merge into IScene once API is nice
	public interface IScene2 : IScene
	{
		string GetName();
		CommonStates[] GetActors();
		SkeletonAnimation GetSkelAnimation();
	}
}

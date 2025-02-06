using System.Collections;
using HFramework.Performer;
using Spine.Unity;

namespace HFramework.Scenes
{
	public interface IScene
	{
		IEnumerator Run();
		void SetController(ISceneController controller);
		bool CanContinue();
		void Destroy();
		string GetName();
		CommonStates[] GetActors();
		SkeletonAnimation GetSkelAnimation();
		string ExpandAnimationName(string originalName);
		SexPerformer GetPerformer();
	}
}

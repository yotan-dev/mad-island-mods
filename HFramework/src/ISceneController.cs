using System.Collections;
using HFramework.Scenes;
using Spine.Unity;

namespace HFramework
{
	public interface ISceneController
	{
		void SetScene(IScene scene);

		IScene GetScene();

		bool IsAnimRunning();

		IEnumerator LoopAnimation(string name);
		
		void LoopAnimationBg(string name);
		
		IEnumerator LoopAnimation(IScene scene, SkeletonAnimation tmpSexAnim, string name);

		IEnumerator PlayTimedStep(string name, float time);
		
		IEnumerator PlayOnceStep(string name, bool skippable = false);
		
		void PlayOnceStepBg(string name);

		IEnumerator WaitForInput();
	}
}

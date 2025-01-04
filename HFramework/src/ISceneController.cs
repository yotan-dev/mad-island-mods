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

		IEnumerable PlayTimedStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, float time);
		
		IEnumerator PlayTimedStep(string name, float time);
		
		IEnumerable PlayOnceStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, bool skippable = false);
		
		IEnumerator PlayOnceStep(string name, bool skippable = false);
		
		void PlayOnceStepBg(string name);

		IEnumerator PlayUntilInputStep(IScene scene, SkeletonAnimation tmpSexAnim, string name);

		IEnumerator WaitForInput();

		IEnumerable PlayPlayerGrappledStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, CommonStates Player);

		IEnumerable PlayPlayerGrapplesStep(IScene scene, SkeletonAnimation sexAnim, string sexType, CommonStates man, CommonStates girl);
	}
}

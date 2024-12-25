using System.Collections;
using ExtendedHSystem.Scenes;
using Spine.Unity;

namespace ExtendedHSystem
{
	public interface ISceneController
	{
		void SetScene(IScene2 scene);

		IEnumerator LoopAnimation(string name);
		
		IEnumerator LoopAnimation(IScene scene, SkeletonAnimation tmpSexAnim, string name);

		IEnumerable PlayTimedStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, float time);
		
		IEnumerable PlayOnceStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, bool skippable = false);
		
		IEnumerator PlayOnceStep_New(string name, bool skippable = false);
		
		IEnumerator PlayOnceStep_New(IScene scene, SkeletonAnimation tmpSexAnim, string name, bool skippable = false);

		IEnumerable PlayUntilInputStep(IScene scene, SkeletonAnimation tmpSexAnim, string name);

		IEnumerable PlayPlayerGrappledStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, CommonStates Player);

		IEnumerable PlayPlayerGrapplesStep(IScene scene, SkeletonAnimation sexAnim, string sexType, CommonStates man, CommonStates girl);
	}
}

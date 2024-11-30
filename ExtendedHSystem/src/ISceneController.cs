using System.Collections;
using ExtendedHSystem.Scenes;
using Spine.Unity;

namespace ExtendedHSystem
{
	public interface ISceneController
	{
		IEnumerable PlayTimedStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, float time);
		
		IEnumerable PlayOnceStep(IScene scene, SkeletonAnimation tmpSexAnim, string name);

		IEnumerable PlayUntilInputStep(IScene scene, SkeletonAnimation tmpSexAnim, string name);

		IEnumerable PlayPlayerGrappledStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, CommonStates Player);
	}
}

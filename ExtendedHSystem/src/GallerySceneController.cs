using System.Collections;
using ExtendedHSystem.Scenes;
using Spine.Unity;
using UnityEngine;

namespace ExtendedHSystem
{
	public class GallerySceneController: ISceneController
	{
		public IEnumerable PlayTimedStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, float time)
		{
			tmpSexAnim.state.SetAnimation(0, name, true);
			float animTime = time;
			while (animTime >= 0f && scene.CanContinue())
			{
				animTime -= Time.deltaTime;
				yield return false;
			}

			yield return animTime <= 0f;
		}

		public IEnumerable PlayOnceStep(IScene scene, SkeletonAnimation tmpSexAnim, string name)
		{
			tmpSexAnim.state.SetAnimation(0, name, false);
			float animTime = tmpSexAnim.state.GetCurrent(0).AnimationEnd;
			while (animTime >= 0f && scene.CanContinue())
			{
				animTime -= Time.deltaTime;
				yield return false;
			}

			yield return animTime <= 0f;
		}

		public IEnumerable PlayUntilInputStep(IScene scene, SkeletonAnimation tmpSexAnim, string name)
		{
			tmpSexAnim.state.SetAnimation(0, name, true);
			yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
			yield return null;
		}

		public IEnumerable PlayPlayerGrappledStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, CommonStates player)
		{
			yield return null;
			yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
			player.faint = 0.0;

			yield return true;
		}

		public void LoopAnimation(IScene scene, SkeletonAnimation tmpSexAnim, string name)
		{
			tmpSexAnim.state.SetAnimation(0, name, true);
		}
	}
}
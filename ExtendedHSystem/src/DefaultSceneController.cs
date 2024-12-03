using System.Collections;
using ExtendedHSystem.Scenes;
using Spine.Unity;
using UnityEngine;

namespace ExtendedHSystem
{
	public class DefaultSceneController: ISceneController
	{
		public void LoopAnimation(IScene scene, SkeletonAnimation tmpSexAnim, string name)
		{
			tmpSexAnim.state.SetAnimation(0, name, true);
		}

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
			PlayerMove pMove = player.GetComponent<PlayerMove>();
			if (pMove == null) {
				yield return false;
				yield break;
			}

			float faintTime = 1f;
			while (scene.CanContinue() && player.faint > 0.0 && pMove.common.dead != 0)
			{
				faintTime -= Time.deltaTime;
				if (faintTime <= 0f)
				{
					faintTime = 1f;
					player.FaintChange(-5.0, true, true);
				}
				yield return null;
			}

			yield return pMove.common.dead != 0;
		}
	}
}
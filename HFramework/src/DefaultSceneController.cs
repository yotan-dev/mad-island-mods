using System.Collections;
using HFramework.Handlers.Animation;
using HFramework.Scenes;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using YotanModCore;

namespace HFramework
{
	public class DefaultSceneController : ISceneController
	{
		private IScene Scene;

		private SkeletonAnimation SexAnim { get { return this.Scene?.GetSkelAnimation(); } }

		public void SetScene(IScene scene)
		{
			this.Scene = scene;
		}

		public IScene GetScene()
		{
			return this.Scene;
		}

		public bool IsAnimRunning()
		{
			return this.SexAnim.AnimationState.GetCurrent(0).TrackTime < this.SexAnim.state.GetCurrent(0).AnimationEnd;
		}

		public IEnumerator LoopAnimation(string name)
		{
			PLogger.LogDebug($"LoopAnimation: {this.Scene.ExpandAnimationName(name)}");
			yield return new LoopAnimation(this.Scene, this.SexAnim, this.Scene.ExpandAnimationName(name)).Handle();
		}

		public void LoopAnimationBg(string name)
		{
			PLogger.LogDebug($"LoopAnimation: {this.Scene.ExpandAnimationName(name)}");
			this.SexAnim.state.SetAnimation(0, name, true);
		}

		public IEnumerator LoopAnimation(IScene scene, SkeletonAnimation tmpSexAnim, string name)
		{
			yield return new LoopAnimation(scene, tmpSexAnim, name).Handle();
		}

		public IEnumerator PlayTimedStep(string name, float time)
		{
			PLogger.LogDebug($"PlayTimedStep_New: {this.Scene.ExpandAnimationName(name)} for {time}");
			yield return new LoopAnimationForTime(this.Scene, this.SexAnim, this.Scene.ExpandAnimationName(name), time).Handle();
		}

		public IEnumerable PlayTimedStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, float time)
		{
			PLogger.LogDebug($"PlayTimedStep V1: {this.Scene.ExpandAnimationName(name)} for {time}");
			if (tmpSexAnim.skeleton.Data.FindAnimation(name) != null)
				tmpSexAnim.state.SetAnimation(0, name, true);

			float animTime = time;
			while (animTime >= 0f && scene.CanContinue())
			{
				animTime -= Time.deltaTime;
				yield return false;
			}

			yield return animTime <= 0f;
		}

		public IEnumerable PlayOnceStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, bool skipable = false)
		{
			float animTime = 0;
			if (tmpSexAnim.skeleton.Data.FindAnimation(name) != null)
			{
				tmpSexAnim.state.SetAnimation(0, name, false);
				animTime = tmpSexAnim.state.GetCurrent(0).AnimationEnd;
			}

			while (animTime >= 0f && scene.CanContinue())
			{
				if (skipable && Input.GetMouseButtonDown(0))
				{
					yield return true;
					yield break;
				}

				animTime -= Time.deltaTime;
				yield return false;
			}

			yield return animTime <= 0f;
		}

		public IEnumerator PlayOnceStep(string name, bool skipable = false)
		{
			PLogger.LogInfo($"PlayOnceStep_New: {this.Scene.ExpandAnimationName(name)}");
			yield return new PlayAnimationOnce(this.Scene, this.SexAnim, this.Scene.ExpandAnimationName(name), skipable).Handle();
		}

		public void PlayOnceStepBg(string name)
		{
			PLogger.LogInfo($"PlayOnceStepBg: {this.Scene.ExpandAnimationName(name)}");
			this.SexAnim.state.SetAnimation(0, name, true);
		}

		public IEnumerator PlayUntilInputStep(IScene scene, SkeletonAnimation tmpSexAnim, string name)
		{
			if (tmpSexAnim.skeleton.Data.FindAnimation(name) != null)
				tmpSexAnim.state.SetAnimation(0, name, true);

			yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
			yield return true;
		}

		public IEnumerator WaitForInput()
		{
			yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || !this.Scene.CanContinue());
		}

		public IEnumerable PlayPlayerGrappledStep(IScene scene, SkeletonAnimation tmpSexAnim, string name, CommonStates player)
		{
			PlayerMove pMove = player.GetComponent<PlayerMove>();
			if (pMove == null)
			{
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

		private void GetBattleStatus(CommonStates man, CommonStates girl, out float limitTime, out float autoTime, out float tmpDamage)
		{
			float num = 1;
			switch (Managers.mn.skillMN.skills[9])
			{
				case 0:
					num = 1f;
					tmpDamage = 1f;
					autoTime = 100f;
					break;

				case 1:
					tmpDamage = 2f;
					autoTime = 0.5f;
					break;

				case 2:
					num = 1.5f;
					tmpDamage = 2f;
					autoTime = 0.4f;
					break;

				case 3:
					num = 1.5f;
					tmpDamage = 3f;
					autoTime = 0.3f;
					break;

				case 4:
					num = 2f;
					tmpDamage = 3f;
					autoTime = 0.2f;
					break;

				default:
					num = 2f;
					tmpDamage = (float)(4 + Managers.mn.skillMN.skills[9] - 5);
					autoTime = 0.1f;
					break;
			}

			float num2 = 10f * (float)Mathf.Clamp(man.level - girl.level, 0, 10) / 10f * num;
			limitTime = 5f + num2;
		}

		public IEnumerable PlayPlayerGrapplesStep(IScene scene, SkeletonAnimation sexAnim, string sexType, CommonStates man, CommonStates girl)
		{
			this.GetBattleStatus(man, girl, out var limitTime, out var autoTime, out var tmpDamage);

			float tmpAutoTime = autoTime;
			float animTime = limitTime;
			Image breakTime = GameObject.Find("UIFXPool").transform.Find("TimeCircleBack/TimeCircle").GetComponent<Image>();
			breakTime.gameObject.transform.parent.transform.position = man.transform.position + Vector3.up * 2f;
			breakTime.gameObject.transform.parent.gameObject.SetActive(true);
			breakTime.fillAmount = 1f;
			string attackAnimName = sexType + "Attack_attack";
			while (animTime > 0f && girl.faint > 0.0 && scene.CanContinue())
			{
				bool flag = false;
				animTime -= Time.deltaTime;
				breakTime.fillAmount = animTime / limitTime;
				if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
					tmpAutoTime -= Time.deltaTime;

				if (tmpAutoTime <= 0f)
				{
					flag = true;
					tmpAutoTime = autoTime;
					girl.StunDamage(man, tmpDamage, true);
				}

				if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
				{
					flag = true;
					girl.StunDamage(man, tmpDamage, true);
				}
				else if (sexAnim.skeleton.Data.FindAnimation(attackAnimName) != null && sexAnim.AnimationName == attackAnimName && sexAnim.AnimationState.GetCurrent(0).TrackTime >= sexAnim.state.GetCurrent(0).AnimationEnd)
				{
					sexAnim.state.SetAnimation(0, sexType + "Attack_loop", true);
				}

				if (sexAnim.skeleton.Data.FindAnimation(attackAnimName) != null && sexAnim.AnimationName != attackAnimName && flag)
					sexAnim.state.SetAnimation(0, attackAnimName, false);

				yield return null;
			}

			breakTime.gameObject.transform.parent.gameObject.SetActive(false);

			if (animTime <= 0f || man.life <= 0.0 || Input.GetKeyDown(KeyCode.R))
				yield return false;
			else
				yield return true;
		}
	}
}

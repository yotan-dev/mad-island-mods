using System.Collections;
using HFramework.Handlers.Animation;
using HFramework.Scenes;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Extensions;

namespace HFramework
{
	public class DefaultSceneController : ISceneController
	{
		private IScene Scene;

		private float? OriginalScale = null;

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

		private void ResumeAnimation()
		{
			if (this.OriginalScale.HasValue)
			{
				this.SexAnim.timeScale = this.OriginalScale.Value;
				this.OriginalScale = null;
			}
		}

		public IEnumerator LoopAnimation(string name)
		{
			PLogger.LogDebug($"LoopAnimation: {this.Scene.ExpandAnimationName(name)}");
			this.ResumeAnimation();
			yield return new LoopAnimation(this.Scene, this.SexAnim, this.Scene.ExpandAnimationName(name)).Handle();
		}

		public void LoopAnimationBg(string name)
		{
			PLogger.LogDebug($"LoopAnimation: {this.Scene.ExpandAnimationName(name)}");
			name = this.Scene.ExpandAnimationName(name);
			if (this.SexAnim.HasAnimation(name))
				this.SexAnim.state.SetAnimation(0, name, true);

			this.ResumeAnimation();
		}

		public IEnumerator PlayTimedStep(string name, float time)
		{
			PLogger.LogDebug($"PlayTimedStep_New: {this.Scene.ExpandAnimationName(name)} for {time}");
			this.ResumeAnimation();
			yield return new LoopAnimationForTime(this.Scene, this.SexAnim, this.Scene.ExpandAnimationName(name), time).Handle();
		}

		public IEnumerator PlayOnceStep(string name, bool skipable = false)
		{
			PLogger.LogDebug($"PlayOnceStep: {this.Scene.ExpandAnimationName(name)}");
			this.ResumeAnimation();
			yield return new PlayAnimationOnce(this.Scene, this.SexAnim, this.Scene.ExpandAnimationName(name), skipable).Handle();
		}

		public void PlayOnceStepBg(string name)
		{
			PLogger.LogDebug($"PlayOnceStepBg: {this.Scene.ExpandAnimationName(name)}");
			name = this.Scene.ExpandAnimationName(name);
			if (this.SexAnim.HasAnimation(name))
				this.SexAnim.state.SetAnimation(0, name, true);
			this.ResumeAnimation();
		}

		public IEnumerator WaitForInput()
		{
			yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || !this.Scene.CanContinue());
		}

		public void SetLockedAnimation(string name)
		{
			PLogger.LogDebug($"SetLockedAnimation: {this.Scene.ExpandAnimationName(name)}");
			this.SexAnim.state.SetAnimation(0, this.Scene.ExpandAnimationName(name), false);
			this.OriginalScale = this.SexAnim.timeScale;
			this.SexAnim.timeScale = 0f;
		}

		public void SetMainCanvasVisible(bool visible)
		{
			Managers.mn.uiMN.MainCanvasView(visible);
		}

		public void SetGameControllable(bool controllable, bool invincible = true)
		{
			Managers.mn.gameMN.Controlable(controllable, invincible);
		}

		public void SetPlayerVisible(bool visible)
		{
			Managers.mn.gameMN.pMove.PlayerVisible(visible);
		}
	}
}

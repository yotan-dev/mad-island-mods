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

		private bool EnsureAnimation(string name)
		{
			if (name == "")
				return false;

			if (!this.SexAnim.HasAnimation(name))
			{
				PLogger.LogError($"Animation {name} not found");
				return false;
			}

			return true;
		}

		public IEnumerator LoopAnimation(string name)
		{
			name = this.Scene.ExpandAnimationName(name);

			PLogger.LogDebug($"LoopAnimation: {name}");

			this.ResumeAnimation();

			if (this.EnsureAnimation(name))
				yield return new LoopAnimation(this.Scene, this.SexAnim, name).Handle();
		}

		public void LoopAnimationBg(string name)
		{
			name = this.Scene.ExpandAnimationName(name);

			PLogger.LogDebug($"LoopAnimation: {name}");

			if (this.EnsureAnimation(name))
				this.SexAnim.state.SetAnimation(0, name, true);

			this.ResumeAnimation();
		}

		public IEnumerator PlayTimedStep(string name, float time)
		{
			name = this.Scene.ExpandAnimationName(name);

			PLogger.LogDebug($"PlayTimedStep_New: {name} for {time}");
			
			this.ResumeAnimation();
			yield return new LoopAnimationForTime(this.Scene, this.SexAnim, name, time).Handle();
		}

		public IEnumerator PlayOnceStep(string name, bool skipable = false)
		{
			name = this.Scene.ExpandAnimationName(name);

			PLogger.LogDebug($"PlayOnceStep: {name}");
			this.ResumeAnimation();
			yield return new PlayAnimationOnce(this.Scene, this.SexAnim, name, skipable).Handle();
		}

		public void PlayOnceStepBg(string name)
		{
			name = this.Scene.ExpandAnimationName(name);

			PLogger.LogDebug($"PlayOnceStepBg: {name}");

			if (this.EnsureAnimation(name))
				this.SexAnim.state.SetAnimation(0, name, true);
			this.ResumeAnimation();
		}

		public IEnumerator WaitForInput()
		{
			yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || !this.Scene.CanContinue());
		}

		public void SetLockedAnimation(string name)
		{
			name = this.Scene.ExpandAnimationName(name);

			PLogger.LogDebug($"SetLockedAnimation: {this.Scene.ExpandAnimationName(name)}");

			if (this.EnsureAnimation(name))
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

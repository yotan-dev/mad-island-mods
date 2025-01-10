using System.Collections;
using HFramework.Handlers.Animation;
using HFramework.Scenes;
using Spine.Unity;
using UnityEngine;
using YotanModCore.Extensions;

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
			name = this.Scene.ExpandAnimationName(name);
			if (this.SexAnim.HasAnimation(name))
				this.SexAnim.state.SetAnimation(0, name, true);
		}

		public IEnumerator PlayTimedStep(string name, float time)
		{
			PLogger.LogDebug($"PlayTimedStep_New: {this.Scene.ExpandAnimationName(name)} for {time}");
			yield return new LoopAnimationForTime(this.Scene, this.SexAnim, this.Scene.ExpandAnimationName(name), time).Handle();
		}

		public IEnumerator PlayOnceStep(string name, bool skipable = false)
		{
			PLogger.LogDebug($"PlayOnceStep: {this.Scene.ExpandAnimationName(name)}");
			yield return new PlayAnimationOnce(this.Scene, this.SexAnim, this.Scene.ExpandAnimationName(name), skipable).Handle();
		}

		public void PlayOnceStepBg(string name)
		{
			PLogger.LogDebug($"PlayOnceStepBg: {this.Scene.ExpandAnimationName(name)}");
			name = this.Scene.ExpandAnimationName(name);
			if (this.SexAnim.HasAnimation(name))
				this.SexAnim.state.SetAnimation(0, name, true);
		}

		public IEnumerator WaitForInput()
		{
			yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || !this.Scene.CanContinue());
		}
	}
}

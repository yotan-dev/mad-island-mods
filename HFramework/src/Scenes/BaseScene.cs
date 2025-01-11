using System;
using System.Collections;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;

namespace HFramework.Scenes
{
	/// <summary>
	/// Holds some common functionality for all scenes
	/// </summary>
	public abstract class BaseScene : IScene
	{
		protected readonly string SceneName;

		protected SkeletonAnimation CommonAnim;

		protected ISceneController Controller;

		protected SexPerformer Performer;

		protected bool Destroyed = false;

		private string CurrentLongLivedStepName = "";

		public BaseScene(string name)
		{
			this.SceneName = name;
		}

		public virtual void Init(ISceneController controller)
		{
			this.Controller = controller;
			controller.SetScene(this);
		}

		public virtual bool CanContinue()
		{
			return !this.Destroyed;
		}

		public virtual void Destroy()
		{
			this.Destroyed = true;
		}

		public virtual string ExpandAnimationName(string originalName)
		{
			return originalName;
		}

		public virtual string GetName()
		{
			return this.SceneName;
		}

		public virtual SexPerformer GetPerformer()
		{
			return this.Performer;
		}

		public virtual SkeletonAnimation GetSkelAnimation()
		{
			return this.CommonAnim;
		}

		public abstract CommonStates[] GetActors();

		public abstract IEnumerator Run();

		protected IEnumerator EndLongLivedStep()
		{
			if (this.CurrentLongLivedStepName == "")
				yield break;

			yield return HookManager.Instance.RunStepEndHook(this, this.CurrentLongLivedStepName);
			this.CurrentLongLivedStepName = "";
		}

		protected IEnumerator StartLongLivedStep(string stepName, Func<IEnumerator> runner)
		{
			if (this.CurrentLongLivedStepName != "")
				yield return this.EndLongLivedStep();

			if (!this.CanContinue())
				yield break;

			yield return HookManager.Instance.RunStepStartHook(this, stepName);

			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, stepName);
				yield break;
			}

			this.CurrentLongLivedStepName = stepName;

			yield return runner();
		}

		protected IEnumerator RunStep(string stepName, Func<IEnumerator> runner)
		{
			if (this.CurrentLongLivedStepName != "")
				yield return this.EndLongLivedStep();

			if (!this.CanContinue())
				yield break;

			yield return HookManager.Instance.RunStepStartHook(this, stepName);

			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, stepName);
				yield break;
			}

			yield return runner();

			yield return HookManager.Instance.RunStepEndHook(this, stepName);
		}
	}
}
